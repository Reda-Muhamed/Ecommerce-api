// Ecomm.Core.DTOs/IdentityDtos.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecomm.Core.DTOs
{
    public record SignUpDto(string Email, string Password, string? FirstName = null);
    public record SignInDto(string Email, string Password);
    public record ChangePasswordDto(string CurrentPassword, string NewPassword);
    public record DeviceInfoDto
    {
        public DeviceInfoDto(string userAgent, string ipAddress, string deviceId, string sessionId)
        {
            UserAgent = userAgent;
            IpAddress = ipAddress;
            if (Guid.TryParse(deviceId, out var parsedDeviceId))
            {
                this.DeviceId = parsedDeviceId;
            }

            SessionId = sessionId ?? string.Empty;
        }
        public string UserAgent { get; init; }
        public string IpAddress { get; init; }
        public Guid DeviceId { get; set; }
        public string SessionId { get; init; }
    }
    public record ConfirmEmailDto
    {
        public Guid UserId { get; init; }
        public string Token { get; init; } = null!;
    }

    public record ResetPasswordDto
    {
        public Guid UserId { get; init; }
        [Required]
        public string Token { get; init; } = null!;
        [Required]
        public string NewPassword { get; init; } = null!;
    }
    public record ForgotPasswordDto
    {
        [Required]
        public string Email { get; init; } = null!;
    }
    public record RefreshTokensDto
    {
        
        public string RefreshToken { get; init; } = null!;
    }
    public record AuthTokensDto
    {
        public string AccessToken { get; init; } = null!;
        public DateTimeOffset AccessTokenExpiresAt { get; init; }
        public string RefreshToken { get; init; } = null!;
        public DateTimeOffset RefreshTokenExpiresAt { get; init; }
        public Guid DeviceId { get; init; }
    }

    // A simple validation result that carries success + errors
    public class ValidationResult
    {
        public bool IsValid { get; init; }
        public IList<string> Errors { get; init; } = new List<string>();
        public static ValidationResult Success() => new() { IsValid = true };
        public static ValidationResult Fail(List<string> errors) => new() { IsValid = false, Errors = new List<string>(errors) };
    }

    // Generic operation result (success + payload or errors)
    public class Result<T>
    {
        public bool IsSuccess { get; init; }
        public T? Value { get; init; }
        public IList<string> Errors { get; init; } = new List<string>();
        public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };

        public static Result<T> Fail(params string[] errors) => new() { IsSuccess = false, Errors = new List<string>(errors) };
    }
}
