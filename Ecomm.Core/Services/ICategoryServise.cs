using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Category;
using Ecomm.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface ICategoryServise
    {
        Task<Result<Category>> GetByIdAsync(Guid categoryId, CancellationToken ct);
        Task<Result<IReadOnlyCollection<CategoryDto>>> GetAllAsync(CancellationToken ct);

    }
}
