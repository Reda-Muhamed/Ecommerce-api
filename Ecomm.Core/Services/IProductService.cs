using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ecomm.Core.DTOs;
using Ecomm.Core.Common;
using Ecomm.Core.DTOs.Products;
namespace Ecomm.Core.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductListItemDto>> GetAllAsync(GetProductsQueryDto query, CancellationToken cancellationToken = default);
        Task<ProductDetailsDto>GetByIdAsync(Guid id , CancellationToken cancellationToken);
        Task<Result<Guid>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken);
        Task<Result<Guid>>AddVariantAsync(Guid productId, CreateVariantDto dto, CancellationToken cancellationToken);
        Task<Result<bool>> AddVariantImagesAsync(
           Guid productId,
           Guid variantId,
           AddVariantImagesDto dto,
           CancellationToken ct);
    }
}
