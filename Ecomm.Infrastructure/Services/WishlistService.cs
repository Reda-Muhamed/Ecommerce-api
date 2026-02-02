using Ecomm.Core.DTOs;
using Ecomm.Core.Entities;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository wishlistRepository;
        private readonly ICurrentUserService currentUser;
        private readonly IUnitOfWork unitOfWork;

        public WishlistService(
            IWishlistRepository wishlistRepository,
            ICurrentUserService currentUser,
            IUnitOfWork unitOfWork)
        {
            this.wishlistRepository = wishlistRepository;
            this.currentUser = currentUser;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> AddAsync(Guid? productId, Guid? variantId, CancellationToken ct)
        {
            if (productId == null && variantId == null)
                return Result<bool>.Fail("InvalidWishlistTarget");

            var userId = currentUser.UserId ?? Guid.Empty;
            if (userId == Guid.Empty)
                return Result<bool>.Fail("Unauthorized");

            if (await wishlistRepository.ExistsAsync(userId, productId, variantId, ct))
                return Result<bool>.Fail("AlreadyInWishlist");

            var wishlist = new Wishlist
            {
                UserId = userId,
                ProductId = productId,
                ProductVariantId = variantId
            };

            await wishlistRepository.AddAsync(wishlist, ct);
            await unitOfWork.CommitAsync(ct);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> RemoveAsync(Guid wishlistId, CancellationToken ct)
        {
            await wishlistRepository.RemoveAsync(wishlistId, ct);
            await unitOfWork.CommitAsync(ct);
            return Result<bool>.Success(true);
        }

        public async Task<Result<IReadOnlyList<WishlistDto>>> GetMyWishlistAsync(CancellationToken ct)
        {
            var userId = currentUser.UserId ?? Guid.Empty;
            if (userId == Guid.Empty)
                return Result<IReadOnlyList<WishlistDto>>.Fail("Unauthorized");

            var items = await wishlistRepository.GetByUserAsync(userId, ct);

            var result = items.Select(w => new WishlistDto
            {
                WishlistId = w.Id,
                ProductId = w.ProductId,
                ProductVariantId = w.ProductVariantId,
                AddedAt = w.AddedAt
            }).ToList();

            return Result<IReadOnlyList<WishlistDto>>.Success(result);
        }

        
        
    }

}
