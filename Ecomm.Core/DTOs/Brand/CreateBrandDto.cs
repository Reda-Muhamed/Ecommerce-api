using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Brand
{
    public class CreateBrandDto
    {
        public string Name { get; set; } = null!;
        public string ?Description { get; set; }
    }
}
