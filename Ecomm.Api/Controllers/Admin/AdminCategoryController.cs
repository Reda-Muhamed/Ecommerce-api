using Ecomm.Core.Authorization;
using Ecomm.Core.DTOs.Category;
using Ecomm.Core.Enums;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/categories")]
    [Authorize(Policy = Permissions.Categories.Create)]
    public class AdminCategoryController : ControllerBase
    {
        private readonly IAdminCategoryService categoryService;

        public AdminCategoryController(IAdminCategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCategoryDto dto,
            CancellationToken ct)
        {
            var result = await categoryService.CreateAsync(dto, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return CreatedAtAction(nameof(GetById),
                new { id = result.Value },
                null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateCategoryDto dto,
            CancellationToken ct)
        {
            var result = await categoryService.UpdateAsync(id, dto, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await categoryService.DeleteAsync(id, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await categoryService.GetByIdAsync(id, ct);
            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Value);
        }
    }

}
