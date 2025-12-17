using Ecomm.Core.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    // the repository interface for user-related data operations {CRUD}
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken ct = default);
        Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken ct = default);
        Task<User?> FindByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(User user, CancellationToken ct = default);
        Task UpdateAsync(User user, CancellationToken ct = default);

    }
}
