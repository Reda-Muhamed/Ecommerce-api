using Ecomm.Core.DTOs.Products;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController: ControllerBase
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult>GetAll([FromQuery] GetProductsQueryDto query, CancellationToken ct)
        {
            var products = await productService.GetAllAsync(query,ct);
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

    }
}
