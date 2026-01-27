using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Products
{
    public class CreateAttributeDto
    {
        public string Name { get; set; } = null!;
        public string Type { get; set; } = "enumerable"; // enumerable / text / numeric
        public bool IsFilterable { get; set; }
        public bool IsVariantable { get; set; }
    }
    public class UpdateAttributeDto
    {
        public string Name { get; set; } = null!;
        public bool IsFilterable { get; set; }
        public bool IsVariantable { get; set; }
    }
    public class CreateAttributeValueDto
    {
        public string Value { get; set; } = null!;
        public int SortOrder { get; set; }
    }
    public class UpdateAttributeValueDto
    {
        public string Value { get; set; } = null!;
        public int SortOrder { get; set; }
    }




}
