using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Configurations
{
   
        /// <summary>
        /// Token-related options bound from configuration (appsettings / env).
        /// </summary>
        public record TokenOptions
        {
            public string SecretKey { get; init; } = null!;                
            public string Issuer { get; init; }
            public string Audience { get; init; }
            public int AccessTokenExpirationMinutes { get; init; } = 15;   
            public int RefreshTokenExpirationDays { get; init; } = 7;
             public int EmailVerificationTokenExpirationHours { get; init; } = 24; // Email verification token validity
            public int ClockSkewSeconds { get; init; } = 60;
        }
    

}
