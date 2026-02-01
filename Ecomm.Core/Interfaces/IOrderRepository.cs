using Ecomm.Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface IOrderRepository
    {
        public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct);
        public Task<bool> ProductHasOrdersAsync(Guid productId, CancellationToken cancellationToken);
        public Task<bool> VariantHasOrdersAsync(Guid variantId, CancellationToken ct);
        public Task<bool> AddAsync(Order order, CancellationToken ct);

    }
}
