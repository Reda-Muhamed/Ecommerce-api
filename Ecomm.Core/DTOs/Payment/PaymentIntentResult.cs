using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Payment
{
    public class PaymentIntentResult
    {
        public string PaymentIntentId { get; init; } = null!;
        public string ClientSecret { get; init; } = null!;
    }
}
