using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Configurations
{
    public record SmtpOptions
    {
   
        public string Host { get; init; } = null!;

     
        public int Port { get; init; }

     
        public string? Username { get; init; }

       
        public string? Password { get; init; }

      
        public bool UseSsl { get; init; } = true;

        
        public string FromEmail { get; init; } = null!;

        
        public string FromName { get; init; } = "Ecomm System";
    }
}
