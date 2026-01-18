// Ecomm.Core.Services/ITokenService.cs
using Ecomm.Core.DTOs;
using Ecomm.Core.Entities;
using Ecomm.Core.Entities.User;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Ecomm.Core.Services
{
   
    public interface ITokenService
    {
        
        Task<(string Token, DateTimeOffset ExpiresAt)> GenerateAccessTokenAsync(User user, IEnumerable<string> roles, CancellationToken cancellationToken = default);

        public DateTimeOffset GetConfirmationMailExpiry();
        Task<(string Token, DateTimeOffset ExpiresAt)> GenerateRefreshTokenAsync(CancellationToken cancellationToken = default);
        public DateTimeOffset GetPasswordResetExpiry();
        
            string GenerateRawToken();
        Task<string> HashTokenAsync(string token, CancellationToken cancellationToken = default);

        Task<ClaimsPrincipal?> ValidateAccessTokenAsync(string token, CancellationToken cancellationToken = default);

    }
}
