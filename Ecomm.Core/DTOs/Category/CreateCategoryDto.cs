using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Category
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = null!;
        public Guid? CategoryParentId { get; set; }
        public string? Description { get; set; }
    }

}
