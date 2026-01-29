using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Category;
using Ecomm.Core.Entities.Product;
using Ecomm.Core.Helpers;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IUnitOfWork unitOfWork;

        public AdminCategoryService(
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            this.categoryRepository = categoryRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> CreateAsync(
            CreateCategoryDto dto,
            CancellationToken ct)
        {
            if (await categoryRepository
                .ExistsByNameAsync(dto.Name, dto.CategoryParentId, ct))
                return Result<Guid>.Fail("CategoryNameAlreadyExists");

            if (dto.CategoryParentId.HasValue &&
                !await categoryRepository.ExistsAsync(dto.CategoryParentId.Value, ct))
                return Result<Guid>.Fail("ParentCategoryNotFound");

            var category = new Category
            {
                Name = dto.Name,
                Slug = SlugHelper.Generate(dto.Name),
                ParentCategoryId = dto.CategoryParentId,
                Description = dto.Description,
                IsActive = true
            };

            await unitOfWork.BeginTransactionAsync(ct);
            try
            {
                await categoryRepository.AddAsync(category, ct);
                await unitOfWork.CommitAsync(ct);
                return Result<Guid>.Success(category.Id);
            }
            catch
            {
                await unitOfWork.RollbackAsync(ct);
                return Result<Guid>.Fail("FailedToCreateCategory");
            }
        }

        public async Task<Result<bool>> UpdateAsync(
            Guid categoryId,
            UpdateCategoryDto dto,
            CancellationToken ct)
        {
            var category = await categoryRepository.GetAsync(categoryId, ct);
            if (category == null || category.IsDeleted)
                return Result<bool>.Fail("CategoryNotFound");
            if (dto.ParentId.HasValue)
            {
                category.ParentCategoryId = dto.ParentId;
            }

            category.Name = dto.Name;
            category.Slug = SlugHelper.Generate(dto.Name);
            category.Description = dto.Description;
            
            category.IsActive = dto.IsActive;
            category.UpdatedAt = DateTimeOffset.UtcNow;

            await unitOfWork.BeginTransactionAsync(ct);
            try
            {
                await categoryRepository.UpdateAsync(category, ct);
                await unitOfWork.CommitAsync(ct);
                return Result<bool>.Success(true);
            }
            catch
            {
                await unitOfWork.RollbackAsync(ct);
                return Result<bool>.Fail("FailedToUpdateCategory");
            }
        }

        public async Task<Result<bool>> DeleteAsync(Guid categoryId, CancellationToken ct)
        {
            var category = await categoryRepository.GetAsync(categoryId, ct);
            if (category == null || category.IsDeleted)
                return Result<bool>.Fail("CategoryNotFound");

            if (await categoryRepository.HasChildrenAsync(categoryId, ct))
                return Result<bool>.Fail("CategoryHasChildren");

            if (await categoryRepository.HasProductsAsync(categoryId, ct))
                return Result<bool>.Fail("CategoryHasProducts");

            category.IsDeleted = true;
            category.IsActive = false;
            category.UpdatedAt = DateTimeOffset.UtcNow;

            await unitOfWork.BeginTransactionAsync(ct);
            try
            {
                await categoryRepository.UpdateAsync(category, ct);
                await unitOfWork.CommitAsync(ct);
                return Result<bool>.Success(true);
            }
            catch
            {
                await unitOfWork.RollbackAsync(ct);
                return Result<bool>.Fail("FailedToDeleteCategory");
            }
        }

        public async Task<Result<Category>> GetByIdAsync(Guid categoryId, CancellationToken ct)
        {
            if (categoryId == Guid.Empty)
                return Result<Category>.Fail("InvalidCategoryId");
            var res =  await categoryRepository.GetAsync(categoryId, ct);
            if(res==null) return Result<Category>.Fail("CAtegoryNotFound");
            return Result<Category>.Success(res);

        }
    }

}
