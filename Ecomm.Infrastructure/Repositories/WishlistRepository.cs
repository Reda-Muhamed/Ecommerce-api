using Ecomm.Core.Entities;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly AppDbContext context;

        public WishlistRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid? productId, Guid? variantId, CancellationToken ct)
        {
            return await context.Wishlists.AnyAsync(w =>
                w.UserId == userId &&
                w.ProductId == productId &&
                w.ProductVariantId == variantId, ct);
        }

        public Task AddAsync(Wishlist wishlist, CancellationToken ct)
            => context.Wishlists.AddAsync(wishlist, ct).AsTask();

        public async Task RemoveAsync(Guid wishlistId, CancellationToken ct)
        {
            var entity = await context.Wishlists.FindAsync(new object[] { wishlistId }, ct);
            if (entity != null)
                context.Wishlists.Remove(entity);
        }

        public async Task<IReadOnlyList<Wishlist>> GetByUserAsync(Guid userId, CancellationToken ct)
        {
            return await context.Wishlists
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync(ct);
        }
    }

}
