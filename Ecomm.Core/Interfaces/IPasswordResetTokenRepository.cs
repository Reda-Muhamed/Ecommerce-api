using Ecomm.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface IPasswordResetTokenRepository
    {
     
        Task AddAsync(PasswordResetToken token, CancellationToken ct = default);

        Task Update(PasswordResetToken token, CancellationToken ct = default);
        Task<PasswordResetToken?> FindByHashAsync(string tokenHash, CancellationToken ct = default);

        
        Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default);

        
        Task DeleteAsync(PasswordResetToken token, CancellationToken ct = default);
    }
}
