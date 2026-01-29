using Ecomm.Core.DTOs.Category;
using Ecomm.Core.Entities.Product;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
        {
            return await _context.Categories
                .AnyAsync(c => c.Id == id && !c.IsDeleted, ct);
        }

        public async Task<bool> ExistsByNameAsync(
            string name,
            Guid? parentId,
            CancellationToken ct)
        {
            var normalizedName = name.Trim().ToLower();

            return await _context.Categories.AnyAsync(c =>
                !c.IsDeleted &&
                c.ParentCategoryId == parentId &&
                c.Name.ToLower() == normalizedName,
                ct);
        }

        public async Task<bool> HasProductsAsync(Guid categoryId, CancellationToken ct)
        {
            return await _context.Products
                .AnyAsync(p =>
                    p.CategoryId == categoryId &&
                    !p.IsDeleted,
                    ct);
        }

        public async Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken ct)
        {
            return await _context.Categories
                .AnyAsync(c =>
                    c.ParentCategoryId == categoryId &&
                    !c.IsDeleted,
                    ct);
        }

        public async Task AddAsync(Category category, CancellationToken ct)
        {
            await _context.Categories.AddAsync(category, ct);
        }

        public async Task<Category?> GetAsync(Guid id, CancellationToken ct)
        {
            return await _context.Categories
                .Include(c => c.Children)
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    !c.IsDeleted,
                    ct);
        }

        public async Task<IReadOnlyCollection<CategoryDto>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.ParentCategoryId==null && !c.IsDeleted)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    ParentCategoryId = null,

                    Children = c.Children
                        .Where(ch => !ch.IsDeleted)
                        .Select(ch => new CategoryDto
                        {
                            Id = ch.Id,
                            Name = ch.Name,
                            Slug = ch.Slug,
                            ParentCategoryId = ch.ParentCategoryId
                        })
                        .ToList()
                })
                .ToListAsync(ct);
        }


        public Task UpdateAsync(Category category, CancellationToken ct)
        {
            _context.Categories.Update(category);
            return Task.CompletedTask;
        }
    }
}
