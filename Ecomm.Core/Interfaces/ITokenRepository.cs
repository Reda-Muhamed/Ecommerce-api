using Ecomm.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface ITokenRepository
    {
        Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
        Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task<IEnumerable<RefreshToken>> GetActiveTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default);
        Task RevokeAsync(RefreshToken token, string revokedByIp, string reason, CancellationToken ct = default);
        Task AddAsync(EmailVerificationToken token, CancellationToken cancellationToken = default);

    }

}
