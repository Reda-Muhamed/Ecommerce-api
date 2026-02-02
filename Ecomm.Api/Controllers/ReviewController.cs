using Ecomm.Core.DTOs;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Api.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService reviewService;

        public ReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(CreateReviewDto dto, CancellationToken ct)
        {
            var result = await reviewService.AddReviewAsync(dto, ct);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok();
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetForProduct(Guid productId, CancellationToken ct)
        {
            var result = await reviewService.GetProductReviewsAsync(productId, ct);
            return Ok(result.Value);
        }
    }

}
