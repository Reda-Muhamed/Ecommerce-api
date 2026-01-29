using Ecomm.Core.Authorization;
using Ecomm.Core.DTOs.Brand;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/brands")]
    [Authorize(Permissions.Brands.Create)]
    public class AdminBrandController : ControllerBase
    {
        private readonly IAdminBrandService _service;

        public AdminBrandController(IAdminBrandService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBrandDto dto, CancellationToken ct)
        {
            var result = await _service.CreateAsync(dto, ct);
            if (!result.IsSuccess) return BadRequest(result.Errors);
            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateBrandDto dto, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, dto, ct);
            if (!result.IsSuccess) return BadRequest(result.Errors);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _service.DeleteAsync(id, ct);
            if (!result.IsSuccess) return BadRequest(result.Errors);
            return Ok();
        }
    }

}
