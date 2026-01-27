using Ecomm.Core.Entities.Product;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ecomm.Infrastructure.Repositories
{
    public class AttributeValueRepository : IAttributeValueRepository
    {
        private readonly AppDbContext _context;

        public AttributeValueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AttributeValue value, CancellationToken ct)
        {
            await _context.AttributeValues.AddAsync(value, ct);
        }

        public Task UpdateAsync(AttributeValue value, CancellationToken ct)
        {
            _context.AttributeValues.Update(value);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(AttributeValue value, CancellationToken ct)
        {
            _context.AttributeValues.Remove(value);
            return Task.CompletedTask;
        }

        public async Task<AttributeValue?> GetAsync(Guid id, CancellationToken ct)
        {
            return await _context.AttributeValues
                .FirstOrDefaultAsync(v => v.Id == id, ct);
        }

        public async Task<bool> ExistsAsync(Guid attributeId, string value, CancellationToken ct)
        {
            var normalizedValue = value.Trim().ToLower();

            return await _context.AttributeValues
                .AnyAsync(v =>
                    v.AttributeId == attributeId &&
                    v.Value.ToLower() == normalizedValue,
                    ct);
        }
    }
}
