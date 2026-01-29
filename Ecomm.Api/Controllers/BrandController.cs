using Ecomm.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _service;

        public BrandController(IBrandService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            return Ok(await _service.GetAllAsync(ct));
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
        {
            var brand = await _service.GetBySlugAsync(slug, ct);
            if (brand == null) return NotFound();
            return Ok(brand);
        }
    }

}
