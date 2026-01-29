using Ecomm.Core.Entities.Product;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;

        public BrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
        {
            return await _context.Brands
                .AnyAsync(b => b.Id == id && !b.IsDeleted, ct);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
        {
            var normalized = name.Trim().ToLower();
            return await _context.Brands
                .AnyAsync(b => !b.IsDeleted && b.Name.ToLower() == normalized, ct);
        }

        public async Task<bool> HasProductsAsync(Guid brandId, CancellationToken ct)
        {
            return await _context.Products
                .AnyAsync(p => p.BrandId == brandId && !p.IsDeleted, ct);
        }

        public async Task AddAsync(Brand brand, CancellationToken ct)
        {
            await _context.Brands.AddAsync(brand, ct);
        }

        public async Task<Brand?> GetAsync(Guid id, CancellationToken ct)
        {
            return await _context.Brands
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, ct);
        }

        public async Task<Brand?> GetBySlugAsync(string slug, CancellationToken ct)
        {
            return await _context.Brands
                .FirstOrDefaultAsync(b => b.Slug == slug && !b.IsDeleted && b.IsActive, ct);
        }

        public async Task UpdateAsync(Brand brand, CancellationToken ct)
        {
            _context.Brands.Update(brand);
            await Task.CompletedTask;
        }

        public async Task<List<Brand>> GetAllActiveAsync(CancellationToken ct)
        {
            return await _context.Brands
                .AsNoTracking()
                .Where(b => b.IsActive && !b.IsDeleted)
                .OrderBy(b => b.Name)
                .ToListAsync(ct);
        }
    }

}
