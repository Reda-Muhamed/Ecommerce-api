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
        Task<Result<bool>> DeleteVariantImageAsync(
            Guid productId,
            Guid variantId,
            Guid imageId,
            CancellationToken ct);

        Task<Result<bool>> UpdateVariantImagesAsync(
            Guid productId,
            Guid variantId,
            UpdateVariantImagesDto dto,
            CancellationToken ct);

        Task<Result<bool>> PublishProductAsync(
            Guid productId,
            CancellationToken ct);
        Task<Result<bool>> ApproveProductAsync(
            Guid productId,
            CancellationToken ct);
        Task<Result<bool>> RejectProductAsync(
            Guid productId,
            CancellationToken ct);
        Task<Result<Guid>>UpdateAsync(
            Guid productId,
            UpdateProductDto dto,
            CancellationToken ct);
        Task<Result<bool>> DeleteProductAsync(
            Guid productId,
            CancellationToken ct);
        public Task<Result<Guid>> UpdateVariantAsync(
            Guid productId,
            Guid variantId,
            UpdateVariantDto dto,
            CancellationToken ct);
        public Task<Result<bool>> DeleteVariantAsync(
            Guid productId,
            Guid variantId,
            CancellationToken ct);

    }
}
