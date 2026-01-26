using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface IOrderRepository
    {
        public Task<bool> ProductHasOrdersAsync(Guid productId, CancellationToken cancellationToken);
        public Task<bool> VariantHasOrdersAsync(Guid variantId, CancellationToken ct);

    }
}
