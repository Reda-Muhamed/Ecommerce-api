using Ecomm.Core.Entities;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext context;

        public RefreshTokenRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
        {
            await context.RefreshTokens.AddAsync(token, ct);
        }

        public async Task<RefreshToken?> FindByHashAsync(string tokenHash, CancellationToken ct = default)
        {
            return await context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
        }

        public async Task<IEnumerable<RefreshToken>> GetActiveTokensForUserAsync(Guid userId, CancellationToken ct = default)
        {
            return await context.RefreshTokens
                .Where(x => x.UserId == userId && x.IsActive)
                .ToListAsync(ct);
        }

        public async Task RevokeAsync(RefreshToken token,string replacedBy, CancellationToken ct = default)
        {
            token.RevokedAt = DateTimeOffset.UtcNow;
            token.ReplacedByToken = replacedBy;
            await Task.CompletedTask;
        }

        public async Task RevokeAllForDeviceAsync(Guid userId,Guid deviceId, CancellationToken ct = default)
        {
            var tokens = await context.RefreshTokens
                .Where(x =>
                    x.UserId == userId &&
                    x.RevokedAt == null &&
                    x.DeviceId == deviceId)
                .ToListAsync(ct);

            foreach (var token in tokens)
            {
                token.RevokedAt = DateTimeOffset.UtcNow;
            }
            
            
        }

        public async Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default)
        {
            var tokens = await context.RefreshTokens
                .Where(x => x.UserId == userId && x.RevokedAt == null)
                .ToListAsync(ct);

            foreach (var token in tokens)
            {
                token.RevokedAt = DateTimeOffset.UtcNow;
            }
        }
    }

}
