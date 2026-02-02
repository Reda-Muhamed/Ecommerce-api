using Ecomm.Core.DTOs;
using Ecomm.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface IReviewService
    {
        Task<Result<bool>> AddReviewAsync(CreateReviewDto dto, CancellationToken ct);
        Task<Result<IReadOnlyList<ReviewDto>>> GetProductReviewsAsync(Guid productId, CancellationToken ct);
    }


}
