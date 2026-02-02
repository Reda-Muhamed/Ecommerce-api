using Ecomm.Core.DTOs;
using Ecomm.Core.Entities.Product;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository reviewRepository;
        private readonly ICurrentUserService currentUser;
        private readonly IUnitOfWork unitOfWork;

        public ReviewService(
            IReviewRepository reviewRepository,
            ICurrentUserService currentUser,
            IUnitOfWork unitOfWork)
        {
            this.reviewRepository = reviewRepository;
            this.currentUser = currentUser;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> AddReviewAsync(CreateReviewDto dto, CancellationToken ct)
        {
            var userId = currentUser.UserId ?? Guid.Empty;
            if (userId == Guid.Empty)
                return Result<bool>.Fail("Unauthorized");

            if (await reviewRepository.ExistsAsync(userId, dto.ProductId, ct))
                return Result<bool>.Fail("AlreadyReviewed");

            var review = new Review
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Title = dto.Title,
                Body = dto.Body,
                IsApproved = false
            };

            await reviewRepository.AddAsync(review, ct);
            await unitOfWork.CommitAsync(ct);

            return Result<bool>.Success(true);
        }

        public async Task<Result<IReadOnlyList<ReviewDto>>> GetProductReviewsAsync(Guid productId, CancellationToken ct)
        {
            var reviews = await reviewRepository.GetApprovedByProductAsync(productId, ct);

            var result = reviews.Select(r => new ReviewDto
            {
                Rating = r.Rating,
                Title = r.Title,
                Body = r.Body,
                CreatedAt = r.CreatedAt
            }).ToList();

            return Result<IReadOnlyList<ReviewDto>>.Success(result);
        }
    }

}
