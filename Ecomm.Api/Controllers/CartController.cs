using Ecomm.Core.DTOs.Cart;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.Mvc;
namespace Ecomm.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CartController:ControllerBase
    {
    
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetCart(CancellationToken ct)
        {
            var result = await _cartService.GetCartAsync(ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Value);
        }

   
        [HttpPost("items")]
        public async Task<IActionResult> AddItem(
            [FromBody] AddCartItemDto dto,
            CancellationToken ct)
        {
            var result = await _cartService.AddItemAsync(dto, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Value);
        }


        [HttpPut("items/{itemId:guid}")]
        public async Task<IActionResult> UpdateItem(
            Guid itemId,
            [FromBody] UpdateCartItemDto dto,
            CancellationToken ct)
        {
            var result = await _cartService.UpdateItemAsync(itemId, dto, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Value);
        }

      
        [HttpDelete("items/{itemId:guid}")]
        public async Task<IActionResult> RemoveItem(
            Guid itemId,
            CancellationToken ct)
        {
            var result = await _cartService.RemoveItemAsync(itemId, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { success = true });
        }
        

    }
}
