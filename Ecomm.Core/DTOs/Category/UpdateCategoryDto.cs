using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Category
{
    public class UpdateCategoryDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsActive { get; set; }
    }

}
