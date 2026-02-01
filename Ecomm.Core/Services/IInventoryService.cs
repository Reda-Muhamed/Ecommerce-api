using Ecomm.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface IInventoryService
    {
        Task<Result<bool>> ReserveStockAsync(Guid variantId,int quantity,CancellationToken ct);

        Task ReleaseStockAsync(Guid variantId,int quantity,CancellationToken ct);
        Task CommitStockAsync(Guid variantId, int quantity, CancellationToken ct);
       
        Task ConfirmReservationAsync(Guid orderId, CancellationToken ct);
        Task ReleaseReservationAsync(Guid orderId, CancellationToken ct);
        

    }
}
