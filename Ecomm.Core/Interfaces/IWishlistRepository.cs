using Ecomm.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface IWishlistRepository
    {
        Task<bool> ExistsAsync(Guid userId, Guid? productId, Guid? variantId, CancellationToken ct);
        Task AddAsync(Wishlist wishlist, CancellationToken ct);
        Task RemoveAsync(Guid wishlistId, CancellationToken ct);
        Task<IReadOnlyList<Wishlist>> GetByUserAsync(Guid userId, CancellationToken ct);
    }

}
