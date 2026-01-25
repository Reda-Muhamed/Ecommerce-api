using Ecomm.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface ISellerRepository
    {
        public Task<Guid> GetByUserIdAsync(Guid UserId, CancellationToken ct);
    }
}
