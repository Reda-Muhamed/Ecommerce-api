using Ecomm.Core.Entities.Cart;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface ICartItemRepository
    {
        Task<CartItem?> GetByIdAsync(Guid id, CancellationToken ct);

        Task<CartItem?> GetItemAsync(Guid cartId, Guid variantId, CancellationToken ct);
        Task AddAsync(CartItem item, CancellationToken ct);
        Task UpdateAsync(CartItem item, CancellationToken ct);
        Task DeleteAsync(CartItem item, CancellationToken ct);
    }

}
