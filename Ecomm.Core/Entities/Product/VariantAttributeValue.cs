// VariantAttributeValue.cs
using System;

namespace Ecomm.Core.Entities.Product
{
    /// <summary>
    /// Pivot: which attribute value is assigned to a variant (one entry per attribute per variant).
    /// Unique constraint: (ProductVariantId, AttributeId)
    /// </summary>
    public class VariantAttributeValue
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductVariantId { get; set; }
        public Guid AttributeId { get; set; }
        public Guid AttributeValueId { get; set; }

        public ProductVariant ProductVariant { get; set; } = null!;
        public ProductAttribute Attribute { get; set; } = null!;
        public AttributeValue AttributeValue { get; set; } = null!;
    }
}
