using Ecomm.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface IPasswordService
    {


        /// <summary>
        /// Hash a plain password and return an encoded string to store in DB.
        /// Format: Argon2id$<saltBase64>$<hashBase64>
        /// </summary>
        public string Hash(string plainPassword);

        /// <summary>
        /// Verify a plain password against the encoded stored hash.
        /// Returns true if password matches.
        /// </summary>
        public bool Verify(string plainPassword, string encodedHash);



        public Task<ValidationResult> ValidatePasswordStrengthAsync(string password, CancellationToken cancellationToken = default);
        



    }
}
