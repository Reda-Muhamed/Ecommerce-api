using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Category
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public Guid? ParentCategoryId { get; set; }

        public List<CategoryDto> Children { get; set; } = new();
    }

}
