using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Brand;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface IAdminBrandService
    {
        Task<Result<Guid>> CreateAsync(CreateBrandDto dto, CancellationToken ct);
        Task<Result<bool>> UpdateAsync(Guid brandId, UpdateBrandDto dto, CancellationToken ct);
        Task<Result<bool>> DeleteAsync(Guid brandId, CancellationToken ct);
    }

}
