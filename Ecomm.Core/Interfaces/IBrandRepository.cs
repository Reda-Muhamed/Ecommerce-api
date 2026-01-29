using Ecomm.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface IBrandRepository
    {
        Task<bool> ExistsAsync(Guid id, CancellationToken ct);
        Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
        Task<bool> HasProductsAsync(Guid brandId, CancellationToken ct);

        Task AddAsync(Brand brand, CancellationToken ct);
        Task<Brand?> GetAsync(Guid id, CancellationToken ct);
        Task<Brand?> GetBySlugAsync(string slug, CancellationToken ct);
        Task UpdateAsync(Brand brand, CancellationToken ct);

        Task<List<Brand>> GetAllActiveAsync(CancellationToken ct);
    }

}
