using Ecomm.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface IEmailVerificationTokenRepository
    {
        Task<EmailVerificationToken> AddAsync(EmailVerificationToken token, CancellationToken ct = default);
        Task Delete(EmailVerificationToken token, CancellationToken ct = default);
        Task<EmailVerificationToken?> FindByUserIdAndHashAsync(Guid userId, string tokenHash, CancellationToken ct = default);
        Task Update(EmailVerificationToken token, CancellationToken ct = default);


    }
}
