using Ecomm.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface IAttributeRepository
    {
        Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
        Task AddAsync(ProductAttribute attribute, CancellationToken ct);
        Task<ProductAttribute?> GetAsync(Guid id, CancellationToken ct);
        Task UpdateAsync(ProductAttribute attribute, CancellationToken ct);
    }

}
