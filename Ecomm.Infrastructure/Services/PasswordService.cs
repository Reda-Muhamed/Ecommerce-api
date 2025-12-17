using System;
using System.Security.Cryptography;
using System.Text;
using Ecomm.Core.DTOs;
using Ecomm.Core.Services;
using Isopoh.Cryptography.Argon2;
namespace Ecomm.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly int _saltSize;
        private readonly int _hashSize;
        private readonly int _memoryCost;
        private readonly int _timeCost;
        private readonly int _lanes;
        private readonly int _threads;

        public PasswordService(int saltSize = 16, int hashSize = 32, int memoryCost = 65536,
                              int timeCost = 3, int lanes = 1, int threads = 1)
        {
            _saltSize = saltSize;
            _hashSize = hashSize;
            _memoryCost = memoryCost;
            _timeCost = timeCost;
            _lanes = lanes;
            _threads = threads;
        }

        public string Hash(string password)
        {
            if (password is null) throw new ArgumentNullException(nameof(password));

            var salt = RandomNumberGenerator.GetBytes(_saltSize);
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            try
            {
                var config = new Argon2Config
                {
                    Type = Argon2Type.HybridAddressing, // Argon2id
                    Version = Argon2Version.Nineteen,
                    TimeCost = _timeCost,
                    MemoryCost = _memoryCost,
                    Lanes = _lanes,
                    Threads = _threads,
                    Salt = salt,
                    HashLength = _hashSize,
                    Password = passwordBytes
                };

                using var argon2 = new Argon2(config);
                using var secureHash = argon2.Hash();

                // copy to owned array to reduce lifetime surprises
                var hashBytes = secureHash.Buffer;
                try
                {
                    var encoded = config.EncodeString(hashBytes);
                    return encoded;
                }
                finally
                {
                    if (hashBytes != null)
                        CryptographicOperations.ZeroMemory(hashBytes);
                }
            }
            finally
            {
                CryptographicOperations.ZeroMemory(passwordBytes);
                if (salt != null) CryptographicOperations.ZeroMemory(salt);
            }
        }

        public bool Verify(string password, string storedHash)
        {
            if (password is null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(storedHash)) return false;

            return Argon2.Verify(storedHash, password);
        }

        public Task<ValidationResult> ValidatePasswordStrengthAsync(
     string password,
     CancellationToken cancellationToken = default)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Password must not be empty.");
                return Task.FromResult(ValidationResult.Fail(errors));
            }

            if (password.Length < 8)
                errors.Add("Password must be at least 8 characters long.");

            if (password.Length > 128)
                errors.Add("Password must not exceed 128 characters.");

            if (!password.Any(char.IsLower))
                errors.Add("Password must contain at least one lowercase letter.");

            if (!password.Any(char.IsUpper))
                errors.Add("Password must contain at least one uppercase letter.");

            if (!password.Any(char.IsDigit))
                errors.Add("Password must contain at least one digit.");

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                errors.Add("Password must contain at least one special character.");

            if (password.Any(char.IsWhiteSpace))
                errors.Add("Password must not contain whitespace.");

            if (errors.Count > 0)
                return Task.FromResult(ValidationResult.Fail(errors));

            return Task.FromResult(ValidationResult.Success());
        }

    }

}