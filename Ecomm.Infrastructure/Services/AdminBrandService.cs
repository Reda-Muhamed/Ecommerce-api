using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Brand;
using Ecomm.Core.Entities.Product;
using Ecomm.Core.Helpers;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class AdminBrandService : IAdminBrandService
    {
        private readonly IBrandRepository _brandRepo;
        private readonly IUnitOfWork _uow;

        public AdminBrandService(IBrandRepository brandRepo, IUnitOfWork uow)
        {
            _brandRepo = brandRepo;
            _uow = uow;
        }

        public async Task<Result<Guid>> CreateAsync(CreateBrandDto dto, CancellationToken ct)
        {
            if (await _brandRepo.ExistsByNameAsync(dto.Name, ct))
                return Result<Guid>.Fail("BrandAlreadyExists");

            var brand = new Brand
            {
                Name = dto.Name,
                Description = dto.Description,
                Slug = SlugHelper.Generate(dto.Name),
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _uow.BeginTransactionAsync(ct);
            await _brandRepo.AddAsync(brand, ct);
            await _uow.CommitAsync(ct);

            return Result<Guid>.Success(brand.Id);
        }

        public async Task<Result<bool>> UpdateAsync(Guid brandId, UpdateBrandDto dto, CancellationToken ct)
        {
            var brand = await _brandRepo.GetAsync(brandId, ct);
            if (brand == null)
                return Result<bool>.Fail("BrandNotFound");

            brand.Name = dto.Name;
            brand.Slug = SlugHelper.Generate(dto.Name);
            brand.IsActive = dto.IsActive;
            brand.UpdatedAt = DateTimeOffset.UtcNow;

            await _uow.BeginTransactionAsync(ct);
            await _brandRepo.UpdateAsync(brand, ct);
            await _uow.CommitAsync(ct);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(Guid brandId, CancellationToken ct)
        {
            var brand = await _brandRepo.GetAsync(brandId, ct);
            if (brand == null)
                return Result<bool>.Fail("BrandNotFound");

            if (await _brandRepo.HasProductsAsync(brandId, ct))
                return Result<bool>.Fail("BrandHasProducts");

            brand.IsDeleted = true;
            brand.IsActive = false;

            await _uow.BeginTransactionAsync(ct);
            await _brandRepo.UpdateAsync(brand, ct);
            await _uow.CommitAsync(ct);

            return Result<bool>.Success(true);
        }
    }

}
