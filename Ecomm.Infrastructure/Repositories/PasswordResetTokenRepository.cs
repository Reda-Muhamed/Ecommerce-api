using Ecomm.Core.Entities;
using Ecomm.Core.Interfaces;
using Ecomm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Repositories
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly AppDbContext _appDbContext;
        public PasswordResetTokenRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public Task AddAsync(PasswordResetToken token, CancellationToken ct = default)
        {
            var entityEntry = _appDbContext.PasswordResetTokens.Add(token);
            return Task.CompletedTask;

        }

        public Task DeleteAsync(PasswordResetToken token, CancellationToken ct = default)
        {
            var entityEntry = _appDbContext.PasswordResetTokens.Remove(token);
            return Task.CompletedTask;
        }

        public Task<PasswordResetToken?> FindByHashAsync(string tokenHash, CancellationToken ct = default)
        {
            var token =  _appDbContext.PasswordResetTokens
                
                .FirstOrDefault(t => t.TokenHash == tokenHash);
            return Task.FromResult(token);
        }

        public Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default)
        {
            var tokens = _appDbContext.PasswordResetTokens
                .Where(t => t.UserId == userId);
            _appDbContext.PasswordResetTokens.RemoveRange(tokens);
            return Task.CompletedTask;

        }
        public Task Update(PasswordResetToken token, CancellationToken ct = default)
        {
            _appDbContext.PasswordResetTokens.Update(token);
            return Task.CompletedTask;
        }
    }
}
