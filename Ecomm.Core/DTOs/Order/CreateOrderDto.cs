using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Order
{
    public class CreateOrderDto
    {
        public Guid ShippingAddressId { get; set; }
        public string PaymentMethod { get; set; } = null!;// e.g., "CreditCard", "PayPal" , "Stripe"

    }
    public class CreateOrderResultDto
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public decimal TotalAmount { get; set; }

        // Payment preparation
        public string PaymentIntentId { get; set; } = null!;
        public string PaymentClientSecret { get; set; } = null!;
    }

}

