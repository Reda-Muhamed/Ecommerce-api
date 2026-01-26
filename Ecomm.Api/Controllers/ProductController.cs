using Ecomm.Core.Authorization;
using Ecomm.Core.DTOs.Products;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

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
        [HttpPut("{productId}/variants/{variantId}")]
        [Authorize(Policy = Permissions.Products.Update)]
        public async Task<IActionResult> UpdateVariant(
            Guid productId,
            Guid variantId,
            [FromBody] UpdateVariantDto dto,
            CancellationToken ct)
        {
            var result = await productService.UpdateVariantAsync(productId, variantId, dto, ct);
            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });
            return Ok(new { message = "Variant updated successfully" });
        }

        [HttpDelete("{productId}/variants/{variantId}")]
        [Authorize(Policy = Permissions.Products.Update)]
        public async Task<IActionResult> DeleteVariant(
            Guid productId,
            Guid variantId,
            CancellationToken ct)
        {
            var result = await productService.DeleteVariantAsync(productId, variantId, ct);
            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });
            return Ok(new { message = "Variant deleted successfully" });
        }



        [HttpPost("{productId}/variants/{variantId}/images")]
        [Authorize(Policy = Permissions.Products.Update)]
        public async Task<IActionResult> AddVariantImages(
            Guid productId,
            Guid variantId,
            [FromForm] AddVariantImagesDto dto,
            CancellationToken ct)
        {
            var result = await productService
                .AddVariantImagesAsync(productId, variantId, dto, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Images added successfully" });
        }




        [HttpPost("{productId}/publish")]//seller 
        [Authorize(Policy = Permissions.Products.Update)]
        public async Task<IActionResult> PublishProduct(
            Guid productId,
            CancellationToken ct)
        {
            var result = await productService.PublishProductAsync(productId,ct);
            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });
            return Ok(new { message = "Product published successfully" });
        }



        [HttpPost("{productId}/approve")]
        [Authorize(Policy = Permissions.Products.Approve)]// admin
        public async Task<IActionResult> ApproveProduct(
            Guid productId,
            CancellationToken ct)
        {
            var result = await productService.ApproveProductAsync(productId, ct);
            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });
            return Ok(new { message = "Product approved successfully" });
        }
        
        
        
        [HttpPost("{productId}/reject")]
        [Authorize(Policy = Permissions.Products.Reject)]// admin
        public async Task<IActionResult> RejectProduct(
            Guid productId,
            CancellationToken ct)
        {
            var result = await productService.RejectProductAsync(productId, ct);
            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });
            return Ok(new { message = "Product Rejected successfully" });
        }



        [HttpPut("{id:guid}")]
        [Authorize(Policy = Permissions.Products.Update)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateProductDto dto,
            CancellationToken ct)
        {
            var result = await productService.UpdateAsync(id, dto, ct);
            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });
            return Ok(new { message = "Product updated successfully" });
        }




        [HttpDelete("{id:guid}")]
        [Authorize(Policy = Permissions.Products.Delete)]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken ct)
        {
            var result = await productService.DeleteProductAsync(id, ct);
            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });
            return Ok(new { message = "Product deleted successfully" });
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

       
    }
}
