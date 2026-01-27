using Ecomm.Core.Entities.Product;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Infrastructure.Repositories
{
    public class AttributeRepository : IAttributeRepository
    {
        private readonly AppDbContext _context;

        public AttributeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
        {
            return await _context.ProductAttributes
                .AsNoTracking()
                .AnyAsync(a => a.Name.ToLower() == name.ToLower(), ct);
        }

        public async Task AddAsync(ProductAttribute attribute, CancellationToken ct)
        {
            await _context.ProductAttributes.AddAsync(attribute, ct);
        }

        public async Task<ProductAttribute?> GetAsync(Guid id, CancellationToken ct)
        {
            return await _context.ProductAttributes
                .Include(a => a.Values) // important for admin editing
                .FirstOrDefaultAsync(a => a.Id == id, ct);
        }

        public Task UpdateAsync(ProductAttribute attribute, CancellationToken ct)
        {
            _context.ProductAttributes.Update(attribute);
            return Task.CompletedTask;
        }
    }
}
