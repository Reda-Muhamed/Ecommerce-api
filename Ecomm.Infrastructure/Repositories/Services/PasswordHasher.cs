using System;
using System.Security.Cryptography;
using System.Text;
using Ecomm.Core.Interfaces.Services;
using Isopoh.Cryptography.Argon2;

public class PasswordHasher : IPasswordHasher, IDisposable
{
    private readonly int _saltSize = 16;
    private readonly int _hashSize = 32;
    private readonly int _memoryCost = 65536; // KiB
    private readonly int _timeCost = 3;
    private readonly int _lanes = 1;
    private readonly int _threads = 1;
    private bool _disposed;

    public string Hash(string password)
    {
        if (password is null) throw new ArgumentNullException(nameof(password));

        // prepare arrays
        var salt = RandomNumberGenerator.GetBytes(_saltSize);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        try
        {
            var config = new Argon2Config
            {
                Type = Argon2Type.HybridAddressing, // Argon2id
                Version = Argon2Version.Nineteen,
                TimeCost=_timeCost,
                MemoryCost = _memoryCost,
                Lanes = _lanes,
                Threads = _threads,
                Salt = salt,
                HashLength = _hashSize,
                Password = passwordBytes
            };

            using var argon2 = new Argon2(config);
            using var secureHash = argon2.Hash(); // SecureArray<byte>

            // copy bytes out
            var hashBytes = secureHash.Buffer; // copy reference
            try
            {
                // Encode to standard argon2 string
                var encoded = config.EncodeString(hashBytes);
                return encoded;
            }
            finally
            {
                // zero the hash bytes after encoding
                if (hashBytes != null)
                    CryptographicOperations.ZeroMemory(hashBytes);
            }
        }
        finally
        {
            // zero sensitive buffers
            CryptographicOperations.ZeroMemory(passwordBytes);
            if (salt != null)
                CryptographicOperations.ZeroMemory(salt);
        }
    }

    public bool Verify(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(storedHash) || password is null)
            return false;

        return Argon2.Verify(storedHash, password);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
    }
}
