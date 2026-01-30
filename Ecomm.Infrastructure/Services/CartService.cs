using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Cart;
using Ecomm.Core.Entities.Cart;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly IProductRepository productRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDeviceInfoProvider deviceInfo;
        private readonly ICurrentUserService currentUser;
        private readonly ICartRepository cartRepository;
        private readonly ICartItemRepository cartItemRepository;

        public CartService(IProductRepository productRepository,IUnitOfWork unitOfWork,IDeviceInfoProvider deviceInfo,ICurrentUserService currentUser,ICartRepository cartRepository, ICartItemRepository cartItemRepository)
        {
            this.productRepository = productRepository;
            this.unitOfWork = unitOfWork;
            this.deviceInfo = deviceInfo;
            this.currentUser = currentUser;
            this.cartRepository = cartRepository;
            this.cartItemRepository = cartItemRepository;
        }
        public async Task<Result<CartDto>> AddItemAsync(AddCartItemDto dto, CancellationToken ct)
        {
            if (dto == null || dto.Quantity <= 0)
                return Result<CartDto>.Fail("Invalid request");

            var userId = currentUser.UserId;
            var sessionId = deviceInfo.GetDeviceInfo().SessionId;

            if (userId == null && string.IsNullOrWhiteSpace(sessionId))
                return Result<CartDto>.Fail("Cannot identify user or session");

            //  Load or create cart
            Cart cart;
            bool isExistingCart = false;


            if (userId != null)
            {
                cart = await cartRepository.GetByUserIdAsync(userId.Value, ct);
                if(cart != null)
                {
                      isExistingCart = true;
                }
                else
                {
                    cart = new Cart { UserId = userId.Value, CreatedAt = DateTimeOffset.UtcNow };

                }

            }
            else
            {
                cart = await cartRepository.GetBySessionIdAsync(sessionId!, ct);
                if (cart != null)
                {
                    isExistingCart = true;
                }
                else
                {
                    cart = new Cart { SessionId = sessionId, CreatedAt = DateTimeOffset.UtcNow };

                }

            }

            // Validate variant
            var variant = await productRepository.GetVariantByIdAsync(dto.ProductVariantId, ct);
            if (variant == null || !variant.IsActive)
                return Result<CartDto>.Fail("Product variant not found");

            var product = await productRepository.GetAsync(variant.ProductId, ct);
            if (product == null || product.IsDeleted || !product.IsActive)
                return Result<CartDto>.Fail("Product not available");

            // Stock check
            var existingItem = isExistingCart
                ? await cartItemRepository.GetItemAsync(cart.Id, dto.ProductVariantId, ct)
                : null;
            var requestedQty = dto.Quantity + (existingItem?.Quantity ?? 0);

            if (variant.StockQuantity < requestedQty)
                return Result<CartDto>.Fail("Insufficient stock");

            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                // Persist cart if new
                if (!isExistingCart)
                    await cartRepository.AddAsync(cart, ct);

                // add or update item
                if (existingItem != null)
                {
                    existingItem.Quantity += dto.Quantity;
                    existingItem.AddedAt = DateTimeOffset.UtcNow;
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductVariantId = dto.ProductVariantId,
                        Quantity = dto.Quantity,
                        PriceAtAddition = variant.Price
                    };

                    await cartItemRepository.AddAsync(cartItem, ct);
                    cart.Items.Add(cartItem);
                }

                cart.UpdatedAt = DateTimeOffset.UtcNow;
                 //await cartRepository.UpdateAsync(cart, ct);

                await unitOfWork.CommitAsync(ct);

                
                var cartDto = new CartDto
                {
                    CartId = cart.Id,
                    TotalPrice = cart.Items.Sum(i => i.PriceAtAddition * i.Quantity),
                    Items = cart.Items.Select(i => new CartItemDto
                    {
                        Id = i.Id,
                        ProductVariantId = i.ProductVariantId,
                        Quantity = i.Quantity,
                        Price = i.PriceAtAddition,
                        LineTotal = i.PriceAtAddition * i.Quantity,
                        Sku = i.ProductVariant?.SKU ?? string.Empty,
                    }).ToList(),
                };
                return Result<CartDto>.Success(cartDto);
            }
            catch
            {
                await unitOfWork.RollbackAsync(ct);
                return Result<CartDto>.Fail("Failed to add item to cart");
            }
        }

        public async Task<Result<CartDto>> GetCartAsync(CancellationToken ct)
        {
            var userId = currentUser.UserId;
            var sessionId = deviceInfo.GetDeviceInfo().SessionId;

            Cart? cart = null;

            if (userId != null)
                cart = await cartRepository.GetByUserIdAsync(userId.Value, ct);
            else if (!string.IsNullOrWhiteSpace(sessionId))
                cart = await cartRepository.GetBySessionIdAsync(sessionId, ct);

            if (cart == null)
            {
                return Result<CartDto>.Success(new CartDto
                {
                    CartId = Guid.Empty,
                    Items = new List<CartItemDto>(),
                    TotalPrice = 0
                });
            }

            var dto = new CartDto
            {
                CartId = cart.Id,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    ProductVariantId = i.ProductVariantId,
                    Quantity = i.Quantity,
                    Price = i.PriceAtAddition,
                    LineTotal = i.Quantity * i.PriceAtAddition
                }).ToList(),
                TotalPrice = cart.Items.Sum(i => i.Quantity * i.PriceAtAddition)
            };

            return Result<CartDto>.Success(dto);
        }


        public async Task MergeAnonymousCartAsync(Guid userId, string sessionId, CancellationToken ct)
        {
            var anonCart = await cartRepository.GetBySessionIdAsync(sessionId, ct);
            if (anonCart == null || !anonCart.Items.Any())
                return;

            var userCart = await cartRepository.GetByUserIdAsync(userId, ct);
            var isNewUserCart = false;

            if (userCart == null)
            {
                userCart = new Cart { UserId = userId };
                isNewUserCart = true;
            }

            await unitOfWork.BeginTransactionAsync(ct);

            if (isNewUserCart)
                await cartRepository.AddAsync(userCart, ct);

            foreach (var anonItem in anonCart.Items)
            {
                var existingItem = userCart.Items
                    .FirstOrDefault(i => i.ProductVariantId == anonItem.ProductVariantId);

                if (existingItem != null)
                {
                    existingItem.Quantity += anonItem.Quantity;
                }
                else
                {
                    userCart.Items.Add(new CartItem
                    {
                        ProductVariantId = anonItem.ProductVariantId,
                        Quantity = anonItem.Quantity,
                        PriceAtAddition = anonItem.PriceAtAddition
                    });
                }
            }
            await cartRepository.UpdateAsync(userCart, ct);

            await cartRepository.DeleteAsync(anonCart, ct);

            await unitOfWork.CommitAsync(ct);
        }


        public async Task<Result<bool>> RemoveItemAsync(Guid itemId, CancellationToken ct)
        {
            var item = await cartItemRepository.GetByIdAsync(itemId, ct);
            if (item == null)
                return Result<bool>.Fail("Item not found");

            var cart = await cartRepository.GetByIdAsync(item.CartId, ct);
            if (cart == null)
                return Result<bool>.Fail("Cart not found");

            var userId = currentUser.UserId;
            var sessionId = deviceInfo.GetDeviceInfo().SessionId;

            if (cart.UserId != null)
            {
                if (userId == null || cart.UserId != userId)
                    return Result<bool>.Fail("Unauthorized");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(sessionId) || cart.SessionId != sessionId)
                    return Result<bool>.Fail("Unauthorized");
            }

            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                await cartItemRepository.DeleteAsync(item, ct);

                
                await unitOfWork.CommitAsync(ct);
                return Result<bool>.Success(true);
            }
            catch
            {
                await unitOfWork.RollbackAsync(ct);
                return Result<bool>.Fail("Failed to remove item");
            }
        }



        public async Task<Result<CartDto>> UpdateItemAsync(
                Guid itemId,
                UpdateCartItemDto dto,
                CancellationToken ct)
        {
            if (dto.Quantity < 0)
                return Result<CartDto>.Fail("Invalid quantity");

            var item = await cartItemRepository.GetByIdAsync(itemId, ct);
            if (item == null)
                return Result<CartDto>.Fail("Item not found");

            var cart = await cartRepository.GetByIdAsync(item.CartId, ct);
            if (cart == null)
                return Result<CartDto>.Fail("Cart not found");

            var userId = currentUser.UserId;
            var sessionId = deviceInfo.GetDeviceInfo().SessionId;

            if (cart.UserId != null)
            {
                if (userId == null || cart.UserId != userId)
                    return Result<CartDto>.Fail("Unauthorized");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(sessionId) || cart.SessionId != sessionId)
                    return Result<CartDto>.Fail("Unauthorized");
            }

            var variant = await productRepository.GetVariantByIdAsync(item.ProductVariantId, ct);
            if (variant == null)
                return Result<CartDto>.Fail("Product variant not found");

            if (dto.Quantity > 0 && variant.StockQuantity < dto.Quantity)
                return Result<CartDto>.Fail("Insufficient stock");

            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                if (dto.Quantity == 0)
                {
                    await cartItemRepository.DeleteAsync(item, ct);
                }
                else
                {
                    item.Quantity = dto.Quantity;
                    await cartItemRepository.UpdateAsync(item, ct);
                }

                await unitOfWork.CommitAsync(ct);

                return await GetCartAsync(ct);
            }
            catch
            {
                await unitOfWork.RollbackAsync(ct);
                return Result<CartDto>.Fail("Failed to update cart item");
            }
        }

    }
}
