using Ecomm.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Interfaces
{
    public interface IPaymentTransactionRepository
    {
        Task AddAsync(PaymentTransaction transaction, CancellationToken ct);
    }

}
