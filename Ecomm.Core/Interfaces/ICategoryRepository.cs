using Ecomm.Core.DTOs.Category;
using Ecomm.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface ICategoryRepository
    {
        Task<bool> ExistsAsync(Guid id, CancellationToken ct);
        Task<bool> ExistsByNameAsync(string name, Guid? parentId, CancellationToken ct);
        Task<bool> HasProductsAsync(Guid categoryId, CancellationToken ct);
        Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken ct);

        Task AddAsync(Category category, CancellationToken ct);
        Task<Category?> GetAsync(Guid id, CancellationToken ct);

        public Task<IReadOnlyCollection<CategoryDto>> GetAllAsync(CancellationToken ct);

        Task UpdateAsync(Category category, CancellationToken ct);
    }

}
