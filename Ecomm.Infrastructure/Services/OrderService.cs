using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Order;
using Ecomm.Core.DTOs.Payment;
using Ecomm.Core.Entities;
using Ecomm.Core.Entities.Order;
using Ecomm.Core.Entities.Product;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using Ecomm.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IPaymentTransactionRepository paymentTransactionRepository;
        private readonly IPaymentService paymentService;
        private readonly IInventoryService inventoryService;
        private readonly IOrderRepository orderRepository;
        private readonly IAddressRepository addressRepository;
        private readonly IProductRepository productRepository;
        private readonly ICartItemRepository cartItemRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly ICartRepository cartRepository;
        private readonly IUnitOfWork unitOfWork;

        public OrderService(IPaymentTransactionRepository paymentTransactionRepository,IPaymentService paymentService,IInventoryService inventoryService,IOrderRepository orderRepository,IAddressRepository addressRepository, IProductRepository productRepository,ICartItemRepository cartItemRepository,ICurrentUserService currentUserService,ICartRepository cartRepository,IUnitOfWork unitOfWork)
        {
            this.paymentTransactionRepository = paymentTransactionRepository;
            this.paymentService = paymentService;
            this.inventoryService = inventoryService;
            this.orderRepository = orderRepository;
            this.addressRepository = addressRepository;
            this.productRepository = productRepository;
            this.cartItemRepository = cartItemRepository;
            this.currentUserService = currentUserService;
            this.cartRepository = cartRepository;
            this.unitOfWork = unitOfWork;
        }
  
        public async Task<Result<CreateOrderResultDto>> CreateOrderAsync(CreateOrderDto createOrderDto,CancellationToken cancellationToken)
        {
            if (createOrderDto == null)
                return Result<CreateOrderResultDto>.Fail("InvalidOrderData");

            if (createOrderDto.ShippingAddressId == Guid.Empty)
                return Result<CreateOrderResultDto>.Fail("InvalidShippingAddress");

            var userId = currentUserService.UserId;
            if (userId == null || userId == Guid.Empty)
                return Result<CreateOrderResultDto>.Fail("UserNotAuthenticated");

            var cart = await cartRepository.GetByUserIdAsync(userId.Value, cancellationToken);
            if (cart == null || cart.Items == null || !cart.Items.Any())
                return Result<CreateOrderResultDto>.Fail("CartIsEmpty");

            var address = await addressRepository
                .GetByIdAsync(createOrderDto.ShippingAddressId, cancellationToken);

            if (address == null)
                return Result<CreateOrderResultDto>.Fail("ShippingAddressNotFound");

            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                // Create order root
                var order = new Order
                {
                    UserId = userId,
                    ShippingAddressId = createOrderDto.ShippingAddressId,
                    Status = "Pending",
                    PaymentStatus = "Pending",
                    CreatedAt = DateTimeOffset.UtcNow,
                    OrderNumber = $"ORD-{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}"
                };

                await orderRepository.AddAsync(order, cancellationToken);

                decimal totalAmount = 0;

                // Create immutable order items + reserve stock
                foreach (var item in cart.Items)
                {
                    await inventoryService.ReserveStockAsync(
                        item.ProductVariantId,
                        item.Quantity,
                        cancellationToken);

                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductVariant.ProductId,
                        ProductVariantId = item.ProductVariantId,
                        SKU = item.ProductVariant.SKU,
                        Title = item.ProductVariant.Title,
                        UnitPrice = item.PriceAtAddition,
                        Quantity = item.Quantity,
                        LineTotal = item.PriceAtAddition * item.Quantity,
                        CreatedAt = DateTimeOffset.UtcNow
                    };

                    totalAmount += orderItem.LineTotal;
                    order.Items.Add(orderItem);
                }

                order.TotalAmount = totalAmount;

                // Audit event
                order.Events.Add(new OrderEvent
                {
                    EventType = "OrderCreated",
                    CreatedAt = DateTimeOffset.UtcNow
                });

                // Prepare payment
                var payment = await paymentService.CreatePaymentIntentAsync(
                    order.Id,
                    order.TotalAmount,
                    createOrderDto.PaymentMethod,
                    cancellationToken);

                //  Clear cart AFTER successful order creation
                await cartRepository.ClearAsync(cart.Id, cancellationToken);

                await unitOfWork.CommitAsync(cancellationToken);

                return Result<CreateOrderResultDto>.Success(new CreateOrderResultDto
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    TotalAmount = totalAmount,
                    PaymentIntentId = payment.Value.PaymentIntentId,
                    PaymentClientSecret = payment.Value.ClientSecret
                });
            }
            catch
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return Result<CreateOrderResultDto>.Fail("FailedToCreateOrder");
            }
        }

        public async Task<Result<OrderDetailsDto>> GetByIdAsync(Guid orderId, CancellationToken ct)
        {
            var order =await orderRepository.GetByIdAsync(orderId, ct);
            if (order == null)
                return Result<OrderDetailsDto>.Fail("OrderNotFound");
            var orderDetails = new OrderDetailsDto
            {
                OrderId = orderId,
                TotalAmount = order.TotalAmount,
                OrderNumber=order.OrderNumber,
                PaymentStatus=order.PaymentStatus,
                Status=order.Status,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemDto { 
                    SKU = i.SKU,
                    Title = i.Title!,
                    UnitPrice= i.UnitPrice,
                    Quantity= i.Quantity,
                    LineTotal = i.LineTotal,
                }
               
               ).ToList(),

            };
            return Result<OrderDetailsDto>.Success(orderDetails);
        }
        public async Task<Result<CheckoutSummaryDto>> GetCheckoutSummaryAsync(CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;
            if (userId == null)
                return Result<CheckoutSummaryDto>.Fail("UserNotAuthenticated");

            var cart = await cartRepository.GetByUserIdAsync(userId.Value, cancellationToken);
            if (cart == null || !cart.Items.Any())
                return Result<CheckoutSummaryDto>.Fail("CartIsEmpty");

            var summary = new CheckoutSummaryDto();
            decimal subTotal = 0;

            foreach (var item in cart.Items)
            {
                var variant = await productRepository
                    .GetVariantByIdAsync(item.ProductVariantId, cancellationToken);

                if (variant == null || !variant.IsActive)
                    return Result<CheckoutSummaryDto>.Fail("VariantUnavailable");

                var lineTotal = variant.Price * item.Quantity;
                
                summary.Items.Add(new CheckoutItemDto
                {
                    ProductId = variant.ProductId,
                    VariantId = variant.Id,
                    SKU = variant.SKU,
                    Title = variant.Title!,
                    UnitPrice = variant.Price,
                    Quantity = item.Quantity,
                    LineTotal = lineTotal
                });

                subTotal += lineTotal;
            }

            summary.Subtotal = subTotal;
            summary.ShippingFee = subTotal > 500 ? 0 : 20; 
            summary.TotalAmount = summary.Subtotal + summary.ShippingFee;

            return Result<CheckoutSummaryDto>.Success(summary);
        }


        public async Task<Result<bool>> MarkOrderPaidAsync(Guid orderId,PaymentConfirmationDto dto,CancellationToken ct)
        {
            if (orderId == Guid.Empty || dto == null)
                return Result<bool>.Fail("InvalidRequest");

            var order = await orderRepository.GetByIdAsync(orderId, ct);
            if (order == null)
                return Result<bool>.Fail("OrderNotFound");

            if (order.PaymentStatus == "Paid")
                return Result<bool>.Success(true);

            if (dto.PaidAmount != order.TotalAmount)
                return Result<bool>.Fail("InvalidPaidAmount");

            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                order.PaymentStatus = "Paid";
                order.Status = "Confirmed";
                order.UpdatedAt = DateTimeOffset.UtcNow;

                var transaction = new PaymentTransaction
                {
                    OrderId = order.Id,
                    TransactionId = dto.PaymentIntentId,     // provider id
                    Amount = dto.PaidAmount,
                    Status = "Success",
                    Method = "Card",                          // or map from dto later
                    ProviderResponse = dto.RawPayload,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                var userId = currentUserService.UserId;
                if (userId == null)
                    return Result<bool>.Fail("UserNotAuthenticated");
                var cart = await cartRepository.GetByUserIdAsync(userId.Value, ct);
                if (cart == null)
                    return Result<bool>.Fail("CartNotFound");
                await paymentTransactionRepository.AddAsync(transaction, ct);

                order.Events.Add(new OrderEvent
                {
                    OrderId = order.Id,
                    EventType = "PaymentCaptured",
                    Data = dto.RawPayload,
                    CreatedAt = DateTimeOffset.UtcNow
                });

                await inventoryService.ConfirmReservationAsync(order.Id, ct);

                await cartRepository.ClearAsync(cart.Id, ct);

                await unitOfWork.CommitAsync(ct);

                return Result<bool>.Success(true);
            }
            catch
            {
                await unitOfWork.RollbackAsync(ct);
                return Result<bool>.Fail("FailedToConfirmPayment");
            }
        }


        public async Task<Result<bool>> CancelOrderAsync(Guid orderId, CancellationToken ct)
        {
            var order = await orderRepository.GetByIdAsync(orderId, ct);
            if (order == null)
                return Result<bool>.Fail("OrderNotFound");

            if (order.PaymentStatus == "Paid")
                return Result<bool>.Fail("CannotCancelPaidOrder");

            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                //  Release stock
                foreach (var item in order.Items)
                {
                    await inventoryService.ReleaseStockAsync(
                        item.ProductVariantId!.Value,
                        item.Quantity,
                        ct);
                }

                // Update order
                order.Status = "Cancelled";
                order.PaymentStatus = "Cancelled";
                order.UpdatedAt = DateTimeOffset.UtcNow;

                // Event
                order.Events.Add(new OrderEvent
                {
                    OrderId = order.Id,
                    EventType = "OrderCancelled"
                });

                await unitOfWork.CommitAsync(ct);
                return Result<bool>.Success(true);
            }
            catch
            {
                await unitOfWork.RollbackAsync(ct);
                return Result<bool>.Fail("FailedToCancelOrder");
            }
        }





    }
}
