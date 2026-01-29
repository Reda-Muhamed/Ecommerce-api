using Ecomm.Core.DTOs.Brand;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface IBrandService
    {
        Task<List<BrandDto>> GetAllAsync(CancellationToken ct);
        Task<BrandDto?> GetBySlugAsync(string slug, CancellationToken ct);
    }

}
