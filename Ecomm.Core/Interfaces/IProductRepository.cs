using Ecomm.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Ecomm.Core.Interfaces
{
    public interface IProductRepository
    {
        public Task AddAsync(Product product, CancellationToken cancellationToken);
        public Task<Product?> GetAsync(Guid productId, CancellationToken cancellationToken);
        public string GenerateUniqueSlugAsync(string name, CancellationToken ct);

        public Task<bool> VariantSkuExistsAsync(string sku, CancellationToken cancellationToken);
        public Task<bool> AttributeValueExistsAsync(Guid attrValueId,CancellationToken cancellationToken);
        public Task <Guid> GetAttributeIdByValueIdAsync(Guid attrValueId, CancellationToken cancellationToken);
        public Task AddVariantAsync(ProductVariant variant, CancellationToken cancellationToken);
        Task<int> CountVariantImagesAsync(Guid variantId, CancellationToken ct);
        Task AddImagesAsync(IEnumerable<ProductImage> images, CancellationToken ct);
        Task<ProductVariant?> GetVariantByIdAsync(Guid variantId, CancellationToken ct);
        Task<int> GetNextVariantImageSortOrderAsync(
          Guid variantId,
          CancellationToken ct);


    }
}
