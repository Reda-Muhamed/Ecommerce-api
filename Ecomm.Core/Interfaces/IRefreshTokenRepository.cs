using Ecomm.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ecomm.Core.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token, CancellationToken ct = default);

        Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken ct = default);

        Task<IEnumerable<RefreshToken>> GetActiveTokensForUserAsync(Guid userId, CancellationToken ct = default);

        Task RevokeAsync(RefreshToken token, string replacedBy,CancellationToken ct = default);

        Task RevokeAllForDeviceAsync(Guid userId, Guid deviceId, CancellationToken ct = default);

        Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default);
    }

}
