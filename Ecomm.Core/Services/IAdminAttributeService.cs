using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface IAdminAttributeService
    {
        Task<Result<Guid>> CreateAttributeAsync(CreateAttributeDto dto, CancellationToken ct);
        Task<Result<bool>> UpdateAttributeAsync(Guid attributeId, UpdateAttributeDto dto, CancellationToken ct);

        Task<Result<Guid>> AddValueAsync(Guid attributeId, CreateAttributeValueDto dto, CancellationToken ct);
        Task<Result<bool>> UpdateValueAsync(Guid valueId, UpdateAttributeValueDto dto, CancellationToken ct);
        Task<Result<bool>> DeleteValueAsync(Guid valueId, CancellationToken ct);
    }

}
