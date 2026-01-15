using Ecomm.Core.DTOs;
using Ecomm.Core.Entities;
using Ecomm.Core.Entities.User;
using Ecomm.Core.Enums;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> logger;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly IPasswordService passwordService;
        private readonly IEmailService emailService;
        private readonly ITokenService tokenService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailVerificationTokenRepository emailVerificationTokenRepository;
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IConfiguration configuration;

        public AuthService(ILogger<AuthService> logger,IRefreshTokenRepository refreshTokenRepository,IPasswordService passwordService,IEmailService emailService,ITokenService tokenService,IUnitOfWork unitOfWork, IEmailVerificationTokenRepository emailVerificationTokenRepository,IUserRepository userRepository,IRoleRepository roleRepository,IConfiguration configuration)
        {
            this.logger = logger;
            this.refreshTokenRepository = refreshTokenRepository;
            this.passwordService = passwordService;
            this.emailService = emailService;
            this.tokenService = tokenService;
            this.unitOfWork = unitOfWork;
            this.emailVerificationTokenRepository = emailVerificationTokenRepository;
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.configuration = configuration;
        }
        public Task<bool> CheckPasswordAsync(User user, string password, CancellationToken cancellationToken = default)
        {
            if (user == null || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(user.PasswordHash))
                return Task.FromResult(false);
            var isValid = passwordService.Verify(password, user.PasswordHash);
            return Task.FromResult(isValid);
        }

        public async Task<Result<bool>> ConfirmEmailAsync(Guid userId,string rawToken,CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(rawToken))
                return Result<bool>.Fail("Invalid request");

            try
            {
                var tokenHash = await tokenService.HashTokenAsync(rawToken, cancellationToken);

                var tokenEntity = await emailVerificationTokenRepository
                    .FindByUserIdAndHashAsync(userId, tokenHash, cancellationToken);

                if (tokenEntity == null)
                    return Result<bool>.Fail("Invalid or expired token");

                if (tokenEntity.ExpiresAt < DateTimeOffset.UtcNow)
                    return Result<bool>.Fail("Token expired");

                if (tokenEntity.IsUsed)
                    return Result<bool>.Fail("Token already used");

                var user = await userRepository.FindByIdAsync(userId, cancellationToken);
                if (user == null)
                    return Result<bool>.Fail("User not found");

                await unitOfWork.BeginTransactionAsync(cancellationToken);

                user.IsEmailConfirmed = true;
                user.UpdatedAt = DateTimeOffset.UtcNow;

                tokenEntity.IsUsed = true;
                tokenEntity.UsedAt = DateTimeOffset.UtcNow;

                await userRepository.Update(user, cancellationToken);
                await emailVerificationTokenRepository.Update(tokenEntity, cancellationToken);

                await unitOfWork.CommitAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return Result<bool>.Fail("Failed to confirm email");
            }
        }


        public async Task<Result<User>> CreateUserAsync(SignUpDto signUpDto, CancellationToken cancellationToken = default)
        // Already validated at DTO level, but double-checking here
        {
            if (signUpDto == null)
                return Result<User>.Fail("SignUp data cannot be null");
            
            var nMail = signUpDto.Email?.Trim().ToLowerInvariant();
            if(string.IsNullOrWhiteSpace(nMail))
            
                return Result<User>.Fail("Email cannot be empty");
            
            if (string.IsNullOrWhiteSpace(signUpDto.Password))
                return Result<User>.Fail("Password cannot be empty");
            // check if email already exists
            var emailExists = await userRepository.EmailExistsAsync(nMail, cancellationToken);
            if (emailExists)
                return Result<User>.Fail("Email is already registered");
            // validate password strength
            ValidationResult? passwordValidation = await passwordService.ValidatePasswordStrengthAsync(signUpDto.Password, cancellationToken);
            if (!passwordValidation.IsValid)
                return Result<User>.Fail(passwordValidation.Errors.ToArray());
            cancellationToken.ThrowIfCancellationRequested();
            //hash password 
            var passwordHash = passwordService.Hash(signUpDto.Password);
            // get the role id for customer
            var role = await roleRepository.GetByNameAsync(RolesEnum.Customer, cancellationToken);
            // create user entity
            var user = new User
            {
                Email = nMail,
                FirstName = signUpDto?.FirstName,
                CreatedAt = DateTimeOffset.UtcNow,
                PasswordHash= passwordHash,
                IsEmailConfirmed= false,
                // RoleId: set to a customer by default, adjust as needed
                RoleId = role.Id,
                SecondName = null,
                UpdatedAt = null,
                IsDeleted = false,
                DeletedAt = null,
            };
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                // Add user 
                await userRepository.AddAsync(user, cancellationToken);
                //  Generate the token 
                var rawToken = await tokenService.CreateEmailVerificationTokenAsync(user.Id, cancellationToken);
                
                // Commit DB changes
                await unitOfWork.CommitAsync(cancellationToken);
                // here we need to send a confirmation email with the token to the user

                // build confirmation link (use FrontendBaseUrl from config)
                var frontendBase = configuration["App:ClientUrl"]?.TrimEnd('/') ?? "https://frontend.com";
                var confirmUrl = $"{frontendBase}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(rawToken)}";

                // simple HTML template (customize)
                var html = $@"
                                  <html>
                                    <body>
                                      <p>Hi {user.FirstName ?? string.Empty},</p>
                                      <p>Thanks for creating an account. Click the link below to confirm your email (expires in 24 hours):</p>
                                      <p><a href=""{confirmUrl}"" style=""display:inline-block;padding:10px 14px;background:#2563eb;color:#fff;border-radius:6px;text-decoration:none;"">Confirm email</a></p>
                                      <p>If the button doesn't work, paste this link into your browser:</p>
                                      <p><small>{confirmUrl}</small></p>
                                      <p>Thanks,<br/>Ecomm Team</p>
                                    </body>
                                  </html>";

                // send email (do not roll back on failure)
                try
                {
                    await emailService.SendEmailConfirmationAsync(user.Email, "Confirm your Ecomm email", html, cancellationToken);
                }
                catch (Exception ex)
                {
                    // log the failure, but do NOT rollback user creation
                    logger.LogError(ex, "Failed to send confirmation email to {Email} for user {UserId}", user.Email, user.Id);
                    // Optionally record a metric / enqueue a retry job
                }

                return Result<User>.Success(user);
            }
            catch (DbUpdateException dbEx)
            {
                // Rollback and translate DB unique-constraint into friendly message when possible.
                try { await unitOfWork.RollbackAsync(cancellationToken); } catch {  }

                // Best-effort detection of unique constraint violation
                var message = dbEx.InnerException?.Message ?? dbEx.Message;
                if (!string.IsNullOrEmpty(message) &&
                    (message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ||
                     message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) ||
                     message.Contains("constraint", StringComparison.OrdinalIgnoreCase)))
                {
                    return Result<User>.Fail("EmailAlreadyExists");
                }

                // Fallback generic DB failure
                return Result<User>.Fail("FailedToCreateUser");
            }
            catch (OperationCanceledException)
            {
                // Let caller know it was cancelled
                try { await unitOfWork.RollbackAsync(CancellationToken.None); } catch { }
                return Result<User>.Fail("RequestCancelled");
            }
            catch (Exception)
            {
                try { await unitOfWork.RollbackAsync(cancellationToken); } catch { }
                return Result<User>.Fail("FailedToCreateUser");
            }

        }

        public Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(normalizedEmail))
                return Task.FromResult<User?>(null);
            var nMail = normalizedEmail.Trim().ToLowerInvariant();
            return userRepository.FindByEmailAsync(nMail, cancellationToken);
        }

        public Task<Result<string>> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

       

        public Task<Result<bool>> ResetPasswordAsync(Guid userId, string token, string newPassword, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RevokeAllRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(User user, string newPassword, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<AuthTokensDto>> SignInAsync(SignInDto dto,DeviceInfoDto deviceInfo,CancellationToken cancellationToken = default)
        {
            if(dto == null || deviceInfo == null)
                return Result<AuthTokensDto>.Fail("Invalid request");

            if (deviceInfo.DeviceId == null || deviceInfo.DeviceId == Guid.Empty)
                return Result<AuthTokensDto>.Fail("DeviceId is required");

            var normalizedEmail = dto.Email?.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(normalizedEmail) || string.IsNullOrWhiteSpace(dto.Password))
                return Result<AuthTokensDto>.Fail("Invalid credentials");
           
            
            // get user
            var user = await userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);
            if (user == null)
                return Result<AuthTokensDto>.Fail("Invalid credentials");

            //Verify password first
            var passwordValid = await CheckPasswordAsync(user, dto.Password, cancellationToken);
            if (!passwordValid)
                return Result<AuthTokensDto>.Fail("Invalid credentials");

            //Check email verification after password is valid to avoid user enumeration
            if (!user.IsEmailConfirmed)
                return Result<AuthTokensDto>.Fail("Please verify your email");

            cancellationToken.ThrowIfCancellationRequested();

            var roles = new List<string>();

            Role role = await roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
            if (role == null)
                return Result<AuthTokensDto>.Fail("User role not found");
            roles.Add(role.Name);

            // generate tokens
            var accessTokenResult = await tokenService.GenerateAccessTokenAsync(user, roles, cancellationToken);
            var refreshTokenResult = await tokenService.GenerateRefreshTokenAsync(cancellationToken);
            var hashedRefreshToken = await tokenService.HashTokenAsync(refreshTokenResult.Token, cancellationToken);

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = hashedRefreshToken,
                ExpiresAt = refreshTokenResult.ExpiresAt,
                CreatedAt = DateTimeOffset.UtcNow,
                IpAddress = deviceInfo.IpAddress,
                UserAgent = deviceInfo.UserAgent,
                DeviceId = deviceInfo.DeviceId,
                RevokedAt = null,
                ReplacedByToken = null
            };

            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                // Revoke old tokens for same device
                await refreshTokenRepository.RevokeAllForDeviceAsync(
                    user.Id,                  
                    deviceInfo.DeviceId,
                    cancellationToken);

                // Store new refresh token
                await refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

                await unitOfWork.CommitAsync(cancellationToken);
            }
            catch
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return Result<AuthTokensDto>.Fail("Login failed");
            }

            var authTokensDto = new AuthTokensDto
            {
                AccessToken = accessTokenResult.Token,
                AccessTokenExpiresAt = accessTokenResult.ExpiresAt,
                RefreshToken = refreshTokenResult.Token,
                RefreshTokenExpiresAt = refreshTokenResult.ExpiresAt,
                DeviceId = deviceInfo.DeviceId!
            };

            return Result<AuthTokensDto>.Success(authTokensDto);
        }
        
        public async Task<Result<AuthTokensDto>> RefreshTokensAsync( string refreshToken, DeviceInfoDto deviceInfo, CancellationToken cancellationToken = default)
        {
            if( string.IsNullOrWhiteSpace(refreshToken)|| deviceInfo == null)
                return Result<AuthTokensDto>.Fail("Invalid request");
            if (deviceInfo.DeviceId == Guid.Empty)
                return Result<AuthTokensDto>.Fail("Invalid device");

            cancellationToken.ThrowIfCancellationRequested();

            var hashedToken = await tokenService.HashTokenAsync(refreshToken,cancellationToken);
            RefreshToken? refreshTokenEntity =await refreshTokenRepository.FindByHashAsync(hashedToken,cancellationToken);
           
            if(refreshTokenEntity == null )
                return Result<AuthTokensDto>.Fail("Invalid refresh token");
            if(refreshTokenEntity.RevokedAt != null)
            {
                // revoke ALL TOKEN for the user
                await refreshTokenRepository.RevokeAllForUserAsync(refreshTokenEntity.UserId,cancellationToken);
                return Result<AuthTokensDto>.Fail("Invalid refresh token");
            }
            if(refreshTokenEntity.ExpiresAt < DateTimeOffset.UtcNow)
                return Result<AuthTokensDto>.Fail("Invalid refresh token");
            if (refreshTokenEntity.DeviceId != deviceInfo.DeviceId)
            {
                // possible token theft
                await refreshTokenRepository.RevokeAllForUserAsync(refreshTokenEntity.UserId,cancellationToken);
                return Result<AuthTokensDto>.Fail("Invalid Device");
            }
            User?user = await userRepository.FindByIdAsync(refreshTokenEntity.UserId,cancellationToken);
            if(user == null)
                return Result<AuthTokensDto>.Fail("User not found");
            var roles = new List<string>();
            Role role = await roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
            if(role == null)
                return Result<AuthTokensDto>.Fail("User role not found");
            roles.Add(role.Name);
            var accessTokenResult = await tokenService.GenerateAccessTokenAsync(user, roles, cancellationToken);
            var newRefreshTokenResult = await tokenService.GenerateRefreshTokenAsync(cancellationToken);
            var newHashedRefreshToken = await tokenService.HashTokenAsync(newRefreshTokenResult.Token, cancellationToken);
            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = newHashedRefreshToken,
                ExpiresAt = newRefreshTokenResult.ExpiresAt,
                CreatedAt = DateTimeOffset.UtcNow,
                IpAddress = deviceInfo.IpAddress,
                UserAgent = deviceInfo.UserAgent,
                RevokedAt = null,
                ReplacedByToken = null,
                DeviceId = deviceInfo.DeviceId
            };
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);
                // Revoke old refresh token
                refreshTokenEntity.RevokedAt = DateTimeOffset.UtcNow;
                refreshTokenEntity.ReplacedByToken = newHashedRefreshToken;
                await refreshTokenRepository.RevokeAsync(refreshTokenEntity,newHashedRefreshToken, cancellationToken);
                // Store new refresh token
                await refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);
                await unitOfWork.CommitAsync(cancellationToken);

            }
            catch
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return Result<AuthTokensDto>.Fail("Failed to refresh tokens");
            }
            var authTokensDto = new AuthTokensDto
            {
                AccessToken = accessTokenResult.Token,
                AccessTokenExpiresAt = accessTokenResult.ExpiresAt,
                RefreshToken = newRefreshTokenResult.Token,
                RefreshTokenExpiresAt = newRefreshTokenResult.ExpiresAt
            };
            return Result<AuthTokensDto>.Success(authTokensDto);
        }

    }
}
