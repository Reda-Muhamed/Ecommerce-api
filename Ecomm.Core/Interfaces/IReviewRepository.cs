using Ecomm.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface IReviewRepository
    {
        Task<bool> ExistsAsync(Guid userId, Guid productId, CancellationToken ct);
        Task AddAsync(Review review, CancellationToken ct);
        Task<IReadOnlyList<Review>> GetApprovedByProductAsync(Guid productId, CancellationToken ct);
    }

}
