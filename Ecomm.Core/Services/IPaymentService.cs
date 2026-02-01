using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.Services
{
    public interface IPaymentService
    {
        Task<Result<PaymentIntentResult>> CreatePaymentIntentAsync(
            Guid orderId,
            decimal amount,
            string paymentMethod,
            CancellationToken ct);
    }
}

