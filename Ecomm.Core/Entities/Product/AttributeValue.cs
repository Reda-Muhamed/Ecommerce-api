// AttributeValue.cs
using System;

namespace Ecomm.Core.Entities.Product
{
    public class AttributeValue
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AttributeId { get; set; }
        public string Value { get; set; } = null!; // e.g., "Red", "Large"
        public int SortOrder { get; set; } = 0;

        public ProductAttribute Attribute { get; set; } = null!;
    }
}
