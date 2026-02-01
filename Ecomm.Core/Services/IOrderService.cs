using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Order;
using Ecomm.Core.DTOs.Payment;


namespace Ecomm.Core.Services
{
    public interface IOrderService
    {
        Task<Result<CheckoutSummaryDto>> GetCheckoutSummaryAsync(CancellationToken cancellationToken);
        Task<Result<CreateOrderResultDto>> CreateOrderAsync(CreateOrderDto createtOrderDto, CancellationToken cancellationToken);
        Task<Result<bool>> MarkOrderPaidAsync(Guid orderId,PaymentConfirmationDto dto,CancellationToken ct);
        Task<Result<bool>> CancelOrderAsync(Guid orderId,CancellationToken ct);
        Task<Result<OrderDetailsDto>> GetByIdAsync(
           Guid orderId,
           CancellationToken ct);

    }
}
