using Ecomm.Core.Common;
using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Products;
using Ecomm.Core.Entities.Product;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Interfaces.ImageStorage;
using Ecomm.Core.Services;
using Ecomm.Infrastructure.Data;
using Ecomm.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class ProductSevice : IProductService
    {
        private readonly AppDbContext context;
        private readonly ISellerRepository sellerRepository;
        private readonly IImageStorageService imageStorageService;
        private readonly ICurrentUserService currentUserService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IProductRepository productRepository;
        private readonly IBrandRepository brandRepository;
        private readonly ICategoryRepository categoryRepository;

        public ProductSevice(AppDbContext context ,ISellerRepository sellerRepository,IImageStorageService imageStorageService,ICurrentUserService currentUserService,IUnitOfWork unitOfWork,IProductRepository productRepository, IBrandRepository brandRepository , ICategoryRepository categoryRepository )
        {
            this.context = context;
            this.sellerRepository = sellerRepository;
            this.imageStorageService = imageStorageService;
            this.currentUserService = currentUserService;
            this.unitOfWork = unitOfWork;
            this.productRepository = productRepository;
            this.brandRepository = brandRepository;
            this.categoryRepository = categoryRepository;
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

        public async Task<ProductDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await context.Products
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
                    Images = p.Images
                        .OrderBy(i => i.SortOrder)
                        .Select(i => new ProductImageDto
                        {
                            Url = i.Url,
                            IsPrimary = i.IsPrimary,
                            SortOrder = i.SortOrder
                        })
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

            if (result == null)
                throw new InvalidOperationException("Product not found.");

            return result;
        }

        public async Task<Result<Guid>> CreateAsync(
            CreateProductDto dto,
            CancellationToken ct)
        {
            if (!await categoryRepository.ExistsAsync(dto.CategoryId, ct))
                return Result<Guid>.Fail("CategoryNotFound");

            if (dto.BrandId.HasValue &&
                !await brandRepository.ExistsAsync(dto.BrandId.Value, ct))
                return Result<Guid>.Fail("BrandNotFound");
            var userId = currentUserService.UserId;
           
            var sellerId = await sellerRepository.GetByUserIdAsync((Guid)userId, ct);
            if (sellerId == Guid.Empty)
                return Result<Guid>.Fail("Unauthorized");
            var slug =  productRepository
                .GenerateUniqueSlugAsync(dto.Name, ct);

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                SellerId = sellerId,
                CategoryId = dto.CategoryId,
                BrandId = dto.BrandId,
                IsPublished = false,
                Slug = slug,
                IsActive = false,
                IsDeleted = false,
                CreatedAt = DateTimeOffset.UtcNow,
                PriceMin = null,
                PriceMax = null
            };

            try
            {
                await unitOfWork.BeginTransactionAsync(ct);
                await productRepository.AddAsync(product, ct);
                await unitOfWork.CommitAsync(ct);
                return Result<Guid>.Success(product.Id);
            }
            catch
            {
                await unitOfWork.RollbackAsync(ct);
                return Result<Guid>.Fail("FailedToCreateProduct");
            }
        }

        public async Task<Result<Guid>> AddVariantAsync(
                Guid productId,
                CreateVariantDto dto,
                CancellationToken ct)
        {
            var product = await productRepository.GetAsync(productId, ct);
            if (product == null)
                return Result<Guid>.Fail("ProductNotFound");

            var currentUserId = currentUserService.UserId;
            Guid sellerId = await sellerRepository.GetByUserIdAsync((Guid)currentUserId, ct);
            if (product.SellerId != sellerId)
                return Result<Guid>.Fail("Unauthorized");

            if (product.IsDeleted)
                return Result<Guid>.Fail("ProductDeleted");

            if (await productRepository.VariantSkuExistsAsync(dto.SKU, ct))
                return Result<Guid>.Fail("VariantSkuAlreadyExists");

            var attributeIds = new HashSet<Guid>();

            var variant = new ProductVariant
            {
                ProductId = productId,
                Title = dto.Title,
                SKU = dto.SKU,
                Price = dto.Price,
                CompareAtPrice = dto.CompareAtPrice,
                StockQuantity = dto.StockQuantity,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            foreach (var attrValueId in dto.AttributeValueIds)
            {
                if (!await productRepository.AttributeValueExistsAsync(attrValueId, ct))
                    return Result<Guid>.Fail($"AttributeValueNotFound:{attrValueId}");

                var attributeId =
                    await productRepository.GetAttributeIdByValueIdAsync(attrValueId, ct);

                if (!attributeIds.Add(attributeId))
                    return Result<Guid>.Fail("DuplicateAttribute");

                variant.VariantAttributeValues.Add(new VariantAttributeValue
                {
                    ProductVariantId = variant.Id,
                    AttributeId = attributeId,
                    AttributeValueId = attrValueId
                });
            }

            // Update price range
            product.PriceMin = product.PriceMin.HasValue
                ? Math.Min(product.PriceMin.Value, variant.Price)
                : variant.Price;

            product.PriceMax = product.PriceMax.HasValue
                ? Math.Max(product.PriceMax.Value, variant.Price)
                : variant.Price;

            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                await productRepository.AddVariantAsync(variant, ct);

                await unitOfWork.CommitAsync(ct);
                return Result<Guid>.Success(variant.Id);
            }
            catch
            {
                await unitOfWork.RollbackAsync(ct);
                return Result<Guid>.Fail("FailedToAddVariant");
            }
        }

        public async Task<Result<bool>> AddVariantImagesAsync(
            Guid productId,
            Guid variantId,
            AddVariantImagesDto dto,
            CancellationToken ct)
        {
            if (dto.ImageUrls == null || dto.ImageUrls.Count == 0)
                return Result<bool>.Fail("NoImagesProvided");

            var variant = await productRepository.GetVariantByIdAsync(variantId, ct);
            if (variant == null || variant.ProductId != productId)
                return Result<bool>.Fail("VariantNotFound");

            var product = await productRepository.GetAsync(productId, ct);
            if (product == null)
                return Result<bool>.Fail("ProductNotFound");

            var currentUserId = currentUserService.UserId;
            var sellerId = await sellerRepository.GetByUserIdAsync((Guid)currentUserId, ct);
            if (product.SellerId != sellerId)
                return Result<bool>.Fail("Unauthorized");

            var sortOrder = await productRepository
                .GetNextVariantImageSortOrderAsync(variantId, ct);

            var images = new List<ProductImage>();

            

            
            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                foreach (var file in dto.ImageUrls)
                {
                    var (url, publicId) = await imageStorageService
                        .UploadAsync(file, $"products/{productId}/variants/{variantId}", ct);

                    images.Add(new ProductImage
                    {
                        ProductVariantId = variantId,
                        Url = url,
                        PublicId = publicId,
                        SortOrder = sortOrder++,
                        IsPrimary = sortOrder == 1
                    });
                }

                await productRepository.AddImagesAsync(images, ct);
                await unitOfWork.CommitAsync(ct);

                return Result<bool>.Success(true);
            }
            catch
            {
                await unitOfWork.RollbackAsync(ct);

                // best-effort cleanup
                foreach (var img in images)
                {
                    await imageStorageService.DeleteAsync(img.PublicId, ct);
                }

                return Result<bool>.Fail("FailedToUploadImages");

            }
        }






    }
}
