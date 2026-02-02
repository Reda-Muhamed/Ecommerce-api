using Ecomm.Core.DTOs;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/wishlist")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        this.wishlistService = wishlistService;
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddWishlistDto dto, CancellationToken ct)
    {
        var result = await wishlistService.AddAsync(dto.ProductId, dto.VariantId, ct);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> MyWishlist(CancellationToken ct)
    {
        var result = await wishlistService.GetMyWishlistAsync(ct);
        return Ok(result.Value);
    }

    [HttpDelete("{wishlistId}")]
    public async Task<IActionResult> Remove(Guid wishlistId, CancellationToken ct)
    {
        await wishlistService.RemoveAsync(wishlistId, ct);
        return NoContent();
    }
}
