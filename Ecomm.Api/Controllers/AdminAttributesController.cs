using Ecomm.Core.Authorization;
using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Products;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/attributes")]
    [Authorize(Policy=Permissions.Products.AttributeManage)]
    // add the permissions to the admin work with the permissions 
    public class AdminAttributesController : ControllerBase
    {
        private readonly IAdminAttributeService _service;

        public AdminAttributesController(IAdminAttributeService service)
        {
            _service = service;
        }


        [HttpPost]
        public async Task<IActionResult> CreateAttribute(
            [FromBody] CreateAttributeDto dto,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateAttributeAsync(dto, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return CreatedAtAction(
                null,
                new { attributeId = result.Value },
                new { id = result.Value });
        }

       
        
        [HttpPut("{attributeId:guid}")]
        public async Task<IActionResult> UpdateAttribute(
            Guid attributeId,
            [FromBody] UpdateAttributeDto dto,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAttributeAsync(attributeId, dto, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Attribute updated successfully" });
        }

        


        [HttpPost("{attributeId:guid}/values")]
        public async Task<IActionResult> AddValue(
            Guid attributeId,
            [FromBody] CreateAttributeValueDto dto,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.AddValueAsync(attributeId, dto, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return CreatedAtAction(
                null,
                new { attributeId },
                new { id = result.Value });
        }

        [HttpPut("values/{valueId:guid}")]
        public async Task<IActionResult> UpdateValue(
            Guid valueId,
            [FromBody] UpdateAttributeValueDto dto,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateValueAsync(valueId, dto, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Attribute value updated successfully" });
        }

        [HttpDelete("values/{valueId:guid}")]
        public async Task<IActionResult> DeleteValue(
            Guid valueId,
            CancellationToken ct)
        {
            var result = await _service.DeleteValueAsync(valueId, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Attribute value deleted successfully" });
        }
    }
}
