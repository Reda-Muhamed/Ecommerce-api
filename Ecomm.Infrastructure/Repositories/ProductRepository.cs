using Ecomm.Core.Entities.Product;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext context;

        public ProductRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(Product product, CancellationToken ct)
        {
            await context.Products.AddAsync(product, ct);


        }
        public string GenerateUniqueSlugAsync(string name, CancellationToken ct)
        {
            var slug = name.ToLower().Replace(" ", "-");
            // check if slug exists
            var exists = context.Products.AnyAsync(p => p.Slug == slug, ct);
            if (exists.Result)
            {
                slug += "-" + Guid.NewGuid().ToString().Substring(0, 8);
            }

            return slug;



        }
        public async Task AddVariantAsync(ProductVariant variant, CancellationToken cancellationToken)
        {
            await context.ProductVariants.AddAsync(variant,cancellationToken);
        }

        public async Task<bool> AttributeValueExistsAsync(Guid attrValueId, CancellationToken cancellationToken)
        {
            return await context.AttributeValues
                .AsNoTracking()
                .AnyAsync(av => av.Id == attrValueId, cancellationToken);

        }

        public async Task<Product?> GetAsync (Guid productId, CancellationToken cancellationToken)
        {
            var product =  await context.Products.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
            return product;
        }

        public async Task<Guid> GetAttributeIdByValueIdAsync(Guid attrValueId, CancellationToken cancellationToken)
        {
            var attributeValue =  await context.AttributeValues
                .AsNoTracking()
                .FirstOrDefaultAsync(av => av.Id == attrValueId,cancellationToken);
            return attributeValue?.AttributeId ?? Guid.Empty;


        }

        public async Task<bool> VariantSkuExistsAsync(string sku, CancellationToken cancellationToken)
        {
            return await context.ProductVariants
                .AsNoTracking()
                .AnyAsync(v => v.SKU == sku, cancellationToken);
        }
        public Task<int> CountVariantImagesAsync(Guid variantId, CancellationToken ct)
            => context.ProductImages
            .CountAsync(i => i.ProductVariantId == variantId, ct);

        public Task AddImagesAsync(IEnumerable<ProductImage> images, CancellationToken ct)
            => context.ProductImages.AddRangeAsync(images, ct);
        public async Task<ProductVariant?> GetVariantByIdAsync(Guid variantId, CancellationToken ct)
        {
            return await context.ProductVariants
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == variantId, ct);
        }


        public async Task<int> GetNextVariantImageSortOrderAsync(
            Guid variantId,
            CancellationToken ct)
        {
            var max = await context.ProductImages
                .Where(i => i.ProductVariantId == variantId)
                .MaxAsync(i => (int?)i.SortOrder, ct);

            return (max ?? 0) + 1;
        }

    }



}
