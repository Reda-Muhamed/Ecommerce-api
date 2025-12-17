using Ecomm.Core.Entities;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ecomm.Infrastructure.Repositories
{
    public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
    {
        private readonly AppDbContext _appDbContext;

        public EmailVerificationTokenRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<EmailVerificationToken> AddAsync(EmailVerificationToken token, CancellationToken cancellationToken = default)
        {
            var entry = await _appDbContext.EmailVerificationTokens.AddAsync(token, cancellationToken);
            return entry.Entity;
        }

        public Task DeleteAsync(EmailVerificationToken token, CancellationToken cancellationToken = default)
        {
            _appDbContext.EmailVerificationTokens.Remove(token);
            return Task.CompletedTask;
        }

        
        public async Task<EmailVerificationToken?> FindByUserIdAndHashAsync(Guid userId, string tokenHash, CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(tokenHash))
                return null;

            // Use AsNoTracking for read-only; remove if caller intends to update returned entity.
            return await _appDbContext.EmailVerificationTokens
                .AsNoTracking()
                .Where(t => t.UserId == userId && t.TokenHash == tokenHash)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
