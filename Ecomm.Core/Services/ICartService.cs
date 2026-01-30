using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Cart;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface ICartService
    {
        Task<Result<CartDto>> AddItemAsync(AddCartItemDto dto, CancellationToken ct);
        Task<Result<CartDto>> UpdateItemAsync(Guid itemId, UpdateCartItemDto dto, CancellationToken ct);
        Task<Result<bool>> RemoveItemAsync(Guid itemId, CancellationToken ct);
        Task<Result<CartDto>> GetCartAsync(CancellationToken ct);
        Task MergeAnonymousCartAsync(Guid userId, string sessionId, CancellationToken ct);
    }

}
