using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Payment;
using Ecomm.Core.Entities.User;
using Ecomm.Core.Interfaces;
using Ecomm.Core.Services;
using Ecomm.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Infrastructure.Services
{

    public class PaymentService : IPaymentService
    {
        public async Task<Result<PaymentIntentResult>> CreatePaymentIntentAsync(
            Guid orderId,
            decimal amount,
            string paymentMethod,
            CancellationToken ct)
        {

            var paymentIntentId = $"pi_{Guid.NewGuid():N}";
            var clientSecret = $"secret_{Guid.NewGuid():N}";

            return Result<PaymentIntentResult>.Success(new PaymentIntentResult
            {
                PaymentIntentId = paymentIntentId,
                ClientSecret = clientSecret
            });
        }

       
    }


}
