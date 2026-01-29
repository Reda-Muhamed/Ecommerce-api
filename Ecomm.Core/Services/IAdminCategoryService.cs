using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Category;
using Ecomm.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface IAdminCategoryService
    {
        Task<Result<Guid>> CreateAsync(CreateCategoryDto dto, CancellationToken ct);
        Task<Result<bool>> UpdateAsync(Guid categoryId, UpdateCategoryDto dto, CancellationToken ct);
        Task<Result<bool>> DeleteAsync(Guid categoryId, CancellationToken ct);
        Task<Result<Category>>GetByIdAsync(Guid categoryId, CancellationToken ct);
    }

}
