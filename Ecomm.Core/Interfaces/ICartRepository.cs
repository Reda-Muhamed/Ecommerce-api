using Ecomm.Core.Entities.Cart;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByIdAsync(Guid id, CancellationToken ct);

        Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken ct);
        Task<Cart?> GetBySessionIdAsync(string sessionId, CancellationToken ct);

        Task AddAsync(Cart cart, CancellationToken ct);
        Task UpdateAsync(Cart cart, CancellationToken ct);
        Task DeleteAsync(Cart cart, CancellationToken ct);
    }

}
