using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Payment
{
    public class PaymentConfirmationDto
    {
        public string PaymentIntentId { get; set; } = null!;
        public decimal PaidAmount { get; set; }
        public string Currency { get; set; } = "USD";
        public string RawPayload { get; set; } = null!;
    }
}
