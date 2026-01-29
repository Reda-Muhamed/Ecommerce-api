using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs.Brand
{
    
        public class BrandDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = null!;
            public string Slug { get; set; } = null!;
        }
    

}
