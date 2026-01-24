using Ecomm.Core.Configurations;
using Ecomm.Core.DTOs;
using Ecomm.Core.Entities;
using Ecomm.Core.Entities.User;
using Ecomm.Core.Enums;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using Ecomm.Infrastructure.Repositories;
using Ecomm.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace Ecomm.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        
        private readonly ILogger<AuthService> logger;
        private readonly IPasswordResetTokenRepository passwordResetTokenRepo;
        private readonly ICurrentUserService currentUser;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly IPasswordService passwordService;
        private readonly IEmailService emailService;
        private readonly ITokenService tokenService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailVerificationTokenRepository emailVerificationTokenRepository;
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IConfiguration configuration;

        public AuthService(ILogger<AuthService> logger, IPasswordResetTokenRepository passwordResetTokenRepo, ICurrentUserService currentUser,IRefreshTokenRepository refreshTokenRepository,IPasswordService passwordService,IEmailService emailService,ITokenService tokenService,IUnitOfWork unitOfWork, IEmailVerificationTokenRepository emailVerificationTokenRepository,IUserRepository userRepository,IRoleRepository roleRepository,IConfiguration configuration)
        {
            this.logger = logger;
            this.passwordResetTokenRepo = passwordResetTokenRepo;
            this.currentUser = currentUser;
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
                var rawToken = tokenService.GenerateRawToken();
                var hashedToken = await tokenService.HashTokenAsync(rawToken, cancellationToken);
                var expiry = tokenService.GetConfirmationMailExpiry();
                var emailVerificationToken = new EmailVerificationToken
                {
                    UserId = user.Id,
                    TokenHash = hashedToken,
                    ExpiresAt =expiry,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                
                await emailVerificationTokenRepository.AddAsync(
                    emailVerificationToken,
                    cancellationToken
                );
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



        public async Task<Result<bool>> ResetPasswordAsync(Guid userId,string token,string newPassword,CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty ||
                string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(newPassword))
                return Result<bool>.Fail("Invalid request");

            var hashedToken = await tokenService.HashTokenAsync(token, cancellationToken);

            var passwordResetToken = await passwordResetTokenRepo
                .FindByHashAsync(hashedToken, cancellationToken);

            if (passwordResetToken == null ||
                passwordResetToken.UserId != userId ||
                passwordResetToken.ExpiresAt < DateTimeOffset.UtcNow ||
                passwordResetToken.IsUsed)
                return Result<bool>.Fail("Invalid or expired token");

            var user = await userRepository.FindByIdAsync(userId, cancellationToken);
            if (user == null)
                return Result<bool>.Fail("Invalid request");

            var passwordValidation =
                await passwordService.ValidatePasswordStrengthAsync(newPassword, cancellationToken);

            if (!passwordValidation.IsValid)
                return Result<bool>.Fail(passwordValidation.Errors.ToArray());

            var newHashedPassword = passwordService.Hash(newPassword);

            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                user.PasswordHash = newHashedPassword;
                user.UpdatedAt = DateTimeOffset.UtcNow;

                passwordResetToken.IsUsed = true;
                passwordResetToken.UsedAt = DateTimeOffset.UtcNow;

                await userRepository.Update(user, cancellationToken);
                await passwordResetTokenRepo.Update(passwordResetToken, cancellationToken);

                // revoke all refresh tokens for the user
                await refreshTokenRepository
                    .RevokeAllForUserAsync(user.Id, cancellationToken);

                await unitOfWork.CommitAsync(cancellationToken);
                return Result<bool>.Success(true);
            }
            catch
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return Result<bool>.Fail("Failed to reset password");
            }
        }

        public async Task RevokeAllRefreshTokensForDeviceAsync(Guid deviceId, CancellationToken ct)
        {
            if (deviceId == Guid.Empty)
                throw new ArgumentException("Invalid deviceId");

            var userId = currentUser.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated");
            try
            {
                await unitOfWork.BeginTransactionAsync();
                await refreshTokenRepository.RevokeAllForDeviceAsync(userId, deviceId, ct);
                await unitOfWork.CommitAsync();

            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw new Exception("Something went wrong");
            }
        }

        public async Task RevokeAllRefreshTokensForUserAsync(CancellationToken ct)
        {
            var userId = currentUser.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated");
            try
            {
                await unitOfWork.BeginTransactionAsync();
                await refreshTokenRepository.RevokeAllForUserAsync(userId, ct);
                await unitOfWork.CommitAsync();

            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw new Exception("Something went wrong");
            }
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

            

            Role UserRole = await roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
            if (UserRole == null)
                return Result<AuthTokensDto>.Fail("User role not found");
            var role = UserRole.Name;

            // generate tokens
            var accessTokenResult = await tokenService.GenerateAccessTokenAsync(user, role, cancellationToken);
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
           
            Role userRole = await roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
            if(userRole == null)
                return Result<AuthTokensDto>.Fail("User role not found");
            var role = userRole.Name;
            var accessTokenResult = await tokenService.GenerateAccessTokenAsync(user, role, cancellationToken);
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

        public async Task ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email?.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(normalizedEmail))
                return;

            var user = await userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);
            if (user == null || !user.IsEmailConfirmed)
                return;

            // Generate raw reset token
            var rawToken = tokenService.GenerateRawToken();

            //  Hash token
            var hashedToken = await tokenService.HashTokenAsync(rawToken, cancellationToken);

            //  Calculate expiry
            var expiresAt = tokenService.GetPasswordResetExpiry();

            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                // Revoke old reset tokens
                await passwordResetTokenRepo.RevokeAllForUserAsync(user.Id, cancellationToken);

                // 5️⃣ Store new reset token (HASHED)
                var resetTokenEntity = new PasswordResetToken
                {
                    UserId = user.Id,
                    TokenHash = hashedToken,
                    ExpiresAt = expiresAt,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                await passwordResetTokenRepo.AddAsync(resetTokenEntity, cancellationToken);

                await unitOfWork.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                logger.LogError(ex, "Failed to process forgot password for email {Email}", email);
                return;
            }

            //  Build reset link
            var frontendBase = configuration["App:ClientUrl"]?.TrimEnd('/') ?? "https://frontend.com";
            var resetUrl = $"{frontendBase}/reset-password?userId={user.Id}&token={Uri.EscapeDataString(rawToken)}";

            //  Send email (best effort)
            var html = $@"
                <html>
                  <body>
                    <p>Hi {user.FirstName ?? string.Empty},</p>
                    <p>You requested a password reset. Click the link below (expires in 1 hour):</p>
                    <p><a href=""{resetUrl}"">Reset Password</a></p>
                    <p>If you did not request this, please ignore this email.</p>
                  </body>
                </html>";

            try
            {
                await emailService.SendEmailConfirmationAsync(
                    user.Email,
                    "Reset your Ecomm password",
                    html,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to send password reset email to {Email} for user {UserId}",
                    user.Email, user.Id);
            }
        }


    }
}
