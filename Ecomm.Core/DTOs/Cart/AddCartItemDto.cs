using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Cart
{
    public class AddCartItemDto
    {
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }

}
