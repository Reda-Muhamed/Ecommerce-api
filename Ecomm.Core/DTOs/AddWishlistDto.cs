using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs
{
    public class AddWishlistDto
    {
        public Guid ?ProductId { get; set; }
        public Guid? VariantId { get; set; }
    }
    public class WishlistDto
    {
        public Guid WishlistId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public DateTimeOffset AddedAt { get; set; }
    }

}
