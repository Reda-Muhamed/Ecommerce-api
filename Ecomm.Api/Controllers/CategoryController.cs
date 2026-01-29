using Ecomm.Core.Services;
using Ecomm.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryServise categoryServise;

        public CategoryController(ICategoryServise categoryServise)
        {
            this.categoryServise = categoryServise;
        }
        
        
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await categoryServise.GetByIdAsync(id, ct);
            if (!result.IsSuccess)
            {
                if (result.Errors.Contains("CategoryNotFound"))
                    return NotFound();

                return BadRequest(new { errors = result.Errors });
            }


            return Ok(result.Value);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await categoryServise.GetAllAsync( ct);
            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });
            
            return Ok(result.Value);
        }

    }
}
