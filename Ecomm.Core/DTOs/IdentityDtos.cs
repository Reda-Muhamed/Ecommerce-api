// Ecomm.Core.DTOs/IdentityDtos.cs
using System;
using System.Collections.Generic;

namespace Ecomm.Core.DTOs
{
    public record SignUpDto(string Email, string Password, string? FirstName = null);
    public record SignInDto(string Email, string Password);
    public record ChangePasswordDto(string CurrentPassword, string NewPassword);
    public record DeviceInfoDto(string UserAgent, string IpAddress, IDictionary<string, string>? Headers = null);
    public record ConfirmEmailDto
    {
        public Guid UserId { get; init; }
        public string Token { get; init; } = null!;
    }

    public record AuthTokensDto
    {
        public string AccessToken { get; init; } = null!;
        public DateTimeOffset AccessTokenExpiresAt { get; init; }
        public string RefreshToken { get; init; } = null!;
        public DateTimeOffset RefreshTokenExpiresAt { get; init; }
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
