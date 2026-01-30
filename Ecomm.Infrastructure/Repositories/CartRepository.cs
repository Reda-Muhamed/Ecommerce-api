using Ecomm.Core.Entities.Cart;
using Ecomm.Core.Interfaces;

using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Infrastructure.Repositories
{
  
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);
        }

        public async Task<Cart?> GetBySessionIdAsync(string sessionId, CancellationToken ct)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId, ct);
        }

        public async Task AddAsync(Cart cart, CancellationToken ct)
        {
            await _context.Carts.AddAsync(cart, ct);
        }

        public Task UpdateAsync(Cart cart, CancellationToken ct)
        {
            _context.Carts.Update(cart);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Cart cart, CancellationToken ct)
        {
            _context.Carts.Remove(cart);
            return Task.CompletedTask;
        }
    }

}
