using Ecomm.Core.Entities.Cart;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Ecomm.Infrastructure.Repositories
{
    
    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _context;

        public CartItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartItem?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == id, ct);
        }

        public async Task<CartItem?> GetItemAsync(Guid cartId, Guid variantId, CancellationToken ct)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(
                    ci => ci.CartId == cartId && ci.ProductVariantId == variantId,
                    ct);
        }

        public async Task<int> CountItemsAsync(Guid cartId, CancellationToken ct)
        {
            return await _context.CartItems
                .CountAsync(ci => ci.CartId == cartId, ct);
        }

        public async Task AddAsync(CartItem item, CancellationToken ct)
        {
            await _context.CartItems.AddAsync(item, ct);
        }

        public Task UpdateAsync(CartItem item, CancellationToken ct)
        {
            _context.CartItems.Update(item);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(CartItem item, CancellationToken ct)
        {
            _context.CartItems.Remove(item);
            return Task.CompletedTask;
        }
    }

}
