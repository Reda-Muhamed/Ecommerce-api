using System;
using System.Collections.Generic;
using System.Text;

namespace Ecomm.Core.DTOs
{
  
    public class ReviewDto
    {
    public int Rating { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    }
    public class CreateReviewDto
    {
        public Guid ProductId { get; set; }
        public int Rating { get; set; } 
        public string? Title { get; set; }
        public string? Body { get; set; }
    }


}
