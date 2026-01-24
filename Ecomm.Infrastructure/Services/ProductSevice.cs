using Ecomm.Core.Common;
using Ecomm.Core.DTOs.Products;
using Ecomm.Core.Services;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class ProductSevice : IProductService
    {
        private readonly AppDbContext context;

        public ProductSevice(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<PagedResult<ProductListItemDto>> GetAllAsync(GetProductsQueryDto query, CancellationToken cancellationToken = default)
        {
            var page = query.Page<= 0 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 || query.PageSize>50 ? 20 : query.PageSize;
            var productsQuery = context.Products
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.IsActive);

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.Trim();
                productsQuery = productsQuery.Where(p =>
                    p.Name.ToLower().Contains(searchTerm.ToLower()) ||
                    (p.Description??"").ToLower().Contains(searchTerm.ToLower()));

            }
            if (query.MinPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => 
                p.PriceMin >= query.MinPrice.Value);
            }
            if (query.MaxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p =>
                p.PriceMax <= query.MaxPrice.Value);
            }
            productsQuery = query.SortBy switch
            {
                "price" => query.SortDesc
                    ? productsQuery.OrderByDescending(p => p.PriceMin)
                    : productsQuery.OrderBy(p => p.PriceMin),

                "rating" => query.SortDesc
                    ? productsQuery.OrderByDescending(p => p.AvgRating)
                    : productsQuery.OrderBy(p => p.AvgRating),

                _ => productsQuery.OrderByDescending(p => p.CreatedAt)
            };
            var totalCount = await productsQuery.CountAsync(cancellationToken);

            var items = await productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.PriceMin ?? p.PriceMax ?? 0,
                    AverageRating = p.AvgRating??0,
                    ReviewsCount = p.TotalReviews,
                    PreviewImages = p.Images
                        .OrderBy(i => i.SortOrder)
                        .Take(5)
                        .Select(i => i.Url)
                        .ToList()
                })
                .ToListAsync(cancellationToken);
            return new PagedResult<ProductListItemDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

        }

        public async Task<ProductDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await context.Products
                .AsNoTracking()
                .Where(p =>
                    p.Id == id &&
                    !p.IsDeleted &&
                    p.IsActive)
                .Select(p => new ProductDetailsDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,

                    Brand = p.Brand != null ? p.Brand.Name : null,
                    Category = p.Category != null ? p.Category.Name : null,

                    PriceFrom = p.PriceMin ?? 0,
                    PriceTo = p.PriceMax ?? 0,

                    AverageRating = (double)(p.AvgRating ?? 0),
                    ReviewsCount = p.TotalReviews,

                    Images = (IReadOnlyList<ProductImageDto>)p.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => i.Url)
                        .ToList(),

                    Variants = p.Variants
                        .Where(v => v.IsActive)
                        .Select(v => new ProductVariantDto
                        {
                            Id = v.Id,
                            SKU = v.SKU,
                            Title = v.Title,
                            Price = v.Price,
                            CompareAtPrice = v.CompareAtPrice,

                            IsInStock = v.Inventories
                                .Any(i => i.AvailableQuantity > 0),

                            Attributes = v.VariantAttributeValues
                                .Select(av => new VariantAttributeDto
                                {
                                    Name = av.Attribute.Name,
                                    Value = av.AttributeValue.Value
                                })
                                .ToList(),

                            Images = v.Images
                                .OrderBy(i => i.SortOrder)
                                .Select(i => i.Url)
                                .ToList()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }


    }
}
