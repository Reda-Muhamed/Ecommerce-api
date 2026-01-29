using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Category;
using Ecomm.Core.Entities.Product;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using Ecomm.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class CategoryService : ICategoryServise
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public async Task<Result<IReadOnlyCollection<CategoryDto>>> GetAllAsync(CancellationToken ct)
        {
            var res = await categoryRepository.GetAllAsync(ct);
            if (res == null)
            {
                return Result<IReadOnlyCollection<CategoryDto>>.Fail("CategoriesNOtFound");
            }
            return Result<IReadOnlyCollection<CategoryDto>>.Success(res);
        }

      

        public async Task<Result<Category>> GetByIdAsync(Guid categoryId, CancellationToken ct)
        {
            if (categoryId == Guid.Empty)
                return Result<Category>.Fail("InvalidCategoryId");
            var res = await categoryRepository.GetAsync(categoryId, ct);
            if (res == null) return Result<Category>.Fail("CAtegoryNotFound");
            return Result<Category>.Success(res);

        }
    }
}
