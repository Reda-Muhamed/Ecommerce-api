using Ecomm.Core.DTOs;
using Ecomm.Core.DTOs.Order;
using Ecomm.Core.DTOs.Payment;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize] // all order operations require authenticated user
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

    
        [HttpGet("checkout-summary")]
        public async Task<IActionResult> GetCheckoutSummary(CancellationToken ct)
        {
            var result = await orderService.GetCheckoutSummaryAsync(ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Value);
        }

       
        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            [FromBody] CreateOrderDto dto,
            CancellationToken ct)
        {
            var result = await orderService.CreateOrderAsync(dto, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return CreatedAtAction(
                nameof(GetById),
                new { orderId = result.Value.OrderId },
                result.Value
            );
        }

       
        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetById(Guid orderId, CancellationToken ct)
        {
            var result = await orderService.GetByIdAsync(orderId, ct);

            if (!result.IsSuccess)
                return NotFound(new { errors = result.Errors });

            return Ok(result.Value);
        }

      
        [HttpPost("{orderId:guid}/confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(
            Guid orderId,
            [FromBody] PaymentConfirmationDto dto,
            CancellationToken ct)
        {
            var result = await orderService.MarkOrderPaidAsync(orderId, dto, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Payment confirmed successfully" });
        }

       
        [HttpPost("{orderId:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid orderId, CancellationToken ct)
        {
            var result = await orderService.CancelOrderAsync(orderId, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Order cancelled successfully" });
        }
    }
}
