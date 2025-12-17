using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface IUnitOfWork
    {
        //Task<int> SaveChangesAsync(CancellationToken ct = default);

        Task BeginTransactionAsync(CancellationToken ct = default);

        Task CommitAsync(CancellationToken ct = default);

        Task RollbackAsync(CancellationToken ct = default);
    }
}
