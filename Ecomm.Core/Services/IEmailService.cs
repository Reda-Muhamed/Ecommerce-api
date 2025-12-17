using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default);
    }

}
