using Ecomm.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface IWishlistService
    {
        Task<Result<bool>> AddAsync(Guid? productId, Guid? variantId, CancellationToken ct);
        Task<Result<bool>> RemoveAsync(Guid wishlistId, CancellationToken ct);
        Task<Result<IReadOnlyList<WishlistDto>>> GetMyWishlistAsync(CancellationToken ct);
    }

}
