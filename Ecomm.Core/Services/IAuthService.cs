// Ecomm.Core.Services/IUserService.cs
using Ecomm.Core.DTOs;
using Ecomm.Core.Entities;
using Ecomm.Core.Entities.User;
using System.Threading;
using System.Threading.Tasks;

namespace Ecomm.Core.Services
{
    
    public interface IAuthService
    {
      
        Task<Result<User>> CreateUserAsync(SignUpDto signUpDto, CancellationToken cancellationToken = default);
        Task<Result<AuthTokensDto>> SignInAsync(SignInDto dto,DeviceInfoDto deviceInfo,CancellationToken cancellationToken = default);
        Task<Result<AuthTokensDto>> RefreshTokensAsync( string refreshToken, DeviceInfoDto deviceInfo, CancellationToken cancellationToken = default);

        Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

       
        Task<bool> CheckPasswordAsync(User user, string password, CancellationToken cancellationToken = default);

        
        Task SetPasswordHashAsync(User user, string newPassword, CancellationToken cancellationToken = default);

       
        Task<Result<bool>> ConfirmEmailAsync(Guid userId, string token, CancellationToken cancellationToken = default);

        // Generate email confirmation token to reset password
        Task<Result<string>> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);

       
        Task<Result<bool>> ResetPasswordAsync(Guid userId, string token, string newPassword, CancellationToken cancellationToken = default);

       
        Task RevokeAllRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
