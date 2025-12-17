using Ecomm.Core.Configurations;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using MailKit.Net.Smtp; 
using MimeKit; 
namespace Ecomm.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpOptions _opts; // host, port, username, password, from
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IOptions<SmtpOptions> opts, ILogger<SmtpEmailService> logger)
        {
            _opts = opts.Value;
            _logger = logger;
        }

        public async Task SendEmailConfirmationAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
        {
            var message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress(_opts.FromName, _opts.FromEmail));
            message.To.Add(MimeKit.MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            message.Body = new MimeKit.BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(_opts.Host, _opts.Port, _opts.UseSsl, ct);
            if (!string.IsNullOrWhiteSpace(_opts.Username))
                await client.AuthenticateAsync(_opts.Username, _opts.Password, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
    }

}
