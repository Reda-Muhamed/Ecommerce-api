using Ecomm.Core.DTOs.Brand;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _repo;

        public BrandService(IBrandRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<BrandDto>> GetAllAsync(CancellationToken ct)
        {
            var brands = await _repo.GetAllActiveAsync(ct);
            return brands.Select(b => new BrandDto
            {
                Id = b.Id,
                Name = b.Name,
                Slug = b.Slug
            }).ToList();
        }

        public async Task<BrandDto?> GetBySlugAsync(string slug, CancellationToken ct)
        {
            var brand = await _repo.GetBySlugAsync(slug, ct);
            if (brand == null) return null;

            return new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name,
                Slug = brand.Slug
            };
        }
    }

}
