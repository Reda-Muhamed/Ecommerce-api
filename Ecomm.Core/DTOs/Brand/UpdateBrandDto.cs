using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Brand
{
    public class UpdateBrandDto
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; } 
        public string? Description { get; set; }

    }
}
