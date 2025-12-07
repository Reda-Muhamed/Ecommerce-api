using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces.Services
{
    public interface IPasswordHasher
    {


        /// <summary>
        /// Hash a plain password and return an encoded string to store in DB.
        /// Format: Argon2id$<saltBase64>$<hashBase64>
        /// </summary>
        string Hash(string plainPassword);

        /// <summary>
        /// Verify a plain password against the encoded stored hash.
        /// Returns true if password matches.
        /// </summary>
        bool Verify(string plainPassword, string encodedHash);



    }
}
