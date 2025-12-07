using System;
using System.Collections.Generic;

namespace Ecomm.Core.Entities.Product
{
    public class ProductAttribute
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!; // e.g., Color, Size
        public string Type { get; set; } = "enumerable"; // enumerable/text/numeric
        public bool IsFilterable { get; set; } = false;
        public bool IsVariantable { get; set; } = false; // used to create variants

        public ICollection<AttributeValue> Values { get; set; } = new List<AttributeValue>();
    }
}
