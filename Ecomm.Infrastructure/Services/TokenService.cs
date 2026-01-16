using Ecomm.Core.Configurations;
using Ecomm.Core.Entities;
using Ecomm.Core.Entities.User;
using Ecomm.Core.Enums;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenOptions _options;
        private readonly byte[] _secretKeyBytes;
        private readonly IEmailVerificationTokenRepository emailRepo;

        public TokenService(IOptions<TokenOptions> options ,IEmailVerificationTokenRepository emailRepo)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(_options.SecretKey))
                throw new ArgumentException("TokenOptions.SecretKey must be provided and non-empty.");

            // Ensure secret bytes for signing/hashing
            _secretKeyBytes = Encoding.UTF8.GetBytes(_options.SecretKey);
            this.emailRepo = emailRepo;
        }

        public async Task<string> CreateEmailVerificationTokenAsync(Guid userId, CancellationToken cancellationToken)
        {
            // Generate a random token string
            var buffer = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buffer);
            var token = WebEncoders.Base64UrlEncode(buffer); // URL safe
            var expiresAt = DateTimeOffset.UtcNow.AddHours(_options.EmailVerificationTokenExpirationHours);
            var hashedToken = HashTokenAsync(token, cancellationToken).GetAwaiter().GetResult();

            var emailVerificationToken = new EmailVerificationToken
            {
                UserId = userId,
                TokenHash = hashedToken,
                ExpiresAt = expiresAt
            };
            await emailRepo.AddAsync(emailVerificationToken, cancellationToken);

            return await Task.FromResult(token);

        }

        public Task<(string Token, DateTimeOffset ExpiresAt)> GenerateAccessTokenAsync(User user, IEnumerable<string> roles = null, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var expiresAt = now.AddMinutes(_options.AccessTokenExpirationMinutes);

            // Build claims
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),

                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,
                        DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                        ClaimValueTypes.Integer64),
                };

            // Add role claims individually (so authorization middleware recognizes them)
            if (roles != null && roles.Any())
                foreach (var r in roles)
                {
                    if (!string.IsNullOrWhiteSpace(r))
                        claims.Add(new Claim(ClaimTypes.Role, r));
                }
          else
                claims.Add(new Claim(ClaimTypes.Role, RolesEnum.Customer)); // Default role if none provided

            var key = new SymmetricSecurityKey(_secretKeyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now.UtcDateTime,
                expires: expiresAt.UtcDateTime,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult((tokenString, new DateTimeOffset(expiresAt.UtcDateTime)));
        }

        public Task<(string Token, DateTimeOffset ExpiresAt)> GenerateRefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            // Use cryptographically secure random bytes and produce Base64Url string (URL-safe)
            var buffer = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buffer);

            var token = WebEncoders.Base64UrlEncode(buffer); // URL safe
            var expiresAt = DateTimeOffset.UtcNow.AddDays(_options.RefreshTokenExpirationDays);
            return Task.FromResult((token, expiresAt));
        }

        public Task<string> HashTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));

            // Use HMAC-SHA256 with the server secret so an attacker cannot precompute hashes without the secret.
            using var hmac = new HMACSHA256(_secretKeyBytes);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
            var hashString = WebEncoders.Base64UrlEncode(hash);
            return Task.FromResult(hashString);
        }

        public Task<ClaimsPrincipal?> ValidateAccessTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(token)) return Task.FromResult<ClaimsPrincipal?>(null);

            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_secretKeyBytes),
                ValidateIssuer = true,
                ValidIssuer = _options.Issuer,
                ValidateAudience = true,
                ValidAudience = _options.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(_options.ClockSkewSeconds),
                RequireExpirationTime = true
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Optional: ensure the token uses expected alg
                if (validatedToken is JwtSecurityToken jwt && !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                    return Task.FromResult<ClaimsPrincipal?>(null);

                return Task.FromResult<ClaimsPrincipal?>(principal);
            }
            catch (SecurityTokenException)
            {
                return Task.FromResult<ClaimsPrincipal?>(null);
            }
            catch (Exception)
            {
                return Task.FromResult<ClaimsPrincipal?>(null);
            }
        }
    }
}
