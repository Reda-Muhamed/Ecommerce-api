using Ecomm.Core.Authorization;
using Ecomm.Core.DTOs.Products;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] GetProductsQueryDto query, CancellationToken ct)
        {
            var products = await productService.GetAllAsync(query, ct);
            return Ok(products);
        }
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var product = await productService.GetByIdAsync(id, ct);

            if (product == null)
                return NotFound();

            return Ok(product);
        }
        [HttpPost]
        [Authorize(Policy = Permissions.Products.Create)]
        public async Task<IActionResult> Create(
             [FromBody] CreateProductDto dto,
             CancellationToken ct)
        {
            var result = await productService.CreateAsync(dto, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value },
                null);
        }


        [HttpPost("{productId}/variants")]
        [Authorize(Policy = Permissions.Products.Update)]
        public async Task<IActionResult> AddVariant(
            Guid productId,
            [FromBody] CreateVariantDto dto,
            CancellationToken ct)
        {
            var result = await productService.AddVariantAsync(productId, dto, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return CreatedAtAction(
                // Assuming there is a GetVariant action to retrieve the created variant
                nameof(GetVariant),
                new { productId, variantId = result.Value },
                null);
        }


        [HttpGet("{productId}/variants/{variantId}")]
        public async Task<IActionResult> GetVariant(
            Guid productId,
            Guid variantId,
            CancellationToken ct)
        {
            //var variant = await productService.GetVariantAsync(productId, variantId, ct);

            //if (variant == null)
            //    return NotFound();

            //return Ok(variant);
            return Ok();
        }

        [HttpPost("{productId}/variants/{variantId}/images")]
        [Authorize(Policy = Permissions.Products.Update)]
        public async Task<IActionResult> AddVariantImages(
             Guid productId,
             Guid variantId,
             [FromBody] AddVariantImagesDto dto,
             CancellationToken ct)
        {
            var result = await productService
                .AddVariantImagesAsync(productId, variantId, dto, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Images added successfully" });
        }


    }
}
