using Ecomm.Core.Entities.Product;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext context;

        public ReviewRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid productId, CancellationToken ct)
        {
            return await context.Reviews
                .AnyAsync(r => r.UserId == userId && r.ProductId == productId, ct);
        }

        public Task AddAsync(Review review, CancellationToken ct)
            => context.Reviews.AddAsync(review, ct).AsTask();

        public async Task<IReadOnlyList<Review>> GetApprovedByProductAsync(Guid productId, CancellationToken ct)
        {
            return await context.Reviews
                .AsNoTracking()
                .Where(r => r.ProductId == productId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(ct);
        }
    }

}
