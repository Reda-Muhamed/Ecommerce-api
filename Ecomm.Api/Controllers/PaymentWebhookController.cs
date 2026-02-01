using Ecomm.Core.DTOs.Payment;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ecomm.API.Controllers.Webhooks
{
    [ApiController]
    [Route("api/webhooks/payments")]
    [AllowAnonymous] 
    public class PaymentWebhookController : ControllerBase
    {
        private readonly IOrderService orderService;
        private readonly IConfiguration configuration;
        private readonly ILogger<PaymentWebhookController> logger;

        public PaymentWebhookController(
            IOrderService orderService,
            IConfiguration configuration,
            ILogger<PaymentWebhookController> logger)
        {
            this.orderService = orderService;
            this.configuration = configuration;
            this.logger = logger;
        }

        private static class StripeEventTypes
        {
            public const string PaymentIntentSucceeded = "payment_intent.succeeded";
            public const string PaymentIntentPaymentFailed = "payment_intent.payment_failed";
        }
       
        [HttpPost("stripe")]
        public async Task<IActionResult> StripeWebhook(CancellationToken ct)
        {
            string json;

            using (var reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
            {
                json = await reader.ReadToEndAsync();
            }

            // Verify Stripe signature
            Event stripeEvent;
            try
            {
                var signatureHeader = Request.Headers["Stripe-Signature"];
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signatureHeader,
                    configuration["Stripe:WebhookSecret"]
                );
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Invalid Stripe webhook signature");
                return BadRequest(); 
            }

            
            try
            {
                switch (stripeEvent.Type)
                {
                    case StripeEventTypes.PaymentIntentSucceeded:
                        await HandlePaymentSucceeded(stripeEvent, json, ct);
                        break;

                    case StripeEventTypes.PaymentIntentPaymentFailed:
                        HandlePaymentFailed(stripeEvent);
                        break;

                    default:
                        logger.LogInformation(
                            "Unhandled Stripe event type: {EventType}",
                            stripeEvent.Type);
                        break;
                }

                return Ok(); // Stripe expects 200
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing Stripe webhook");
                return StatusCode(500); // Stripe retries automatically
            }
        }

       
        private async Task HandlePaymentSucceeded(
            Event stripeEvent,
            string rawPayload,
            CancellationToken ct)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

            if (paymentIntent == null)
                throw new StripeException("PaymentIntent missing");

            if (!paymentIntent.Metadata.ContainsKey("orderId"))
                throw new StripeException("orderId missing in metadata");

            var orderId = Guid.Parse(paymentIntent.Metadata["orderId"]);

            var confirmation = new PaymentConfirmationDto
            {
                PaymentIntentId = paymentIntent.Id,
                PaidAmount = paymentIntent.AmountReceived / 100m,
                Currency = paymentIntent.Currency.ToUpper(),
                RawPayload = rawPayload
            };

            var result = await orderService.MarkOrderPaidAsync(orderId, confirmation, ct);

            if (!result.IsSuccess)
            {
                logger.LogWarning(
                    "Failed to mark order {OrderId} as paid: {Errors}",
                    orderId,
                    string.Join(",", result.Errors));
            }
        }

        
        private void HandlePaymentFailed(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

            if (paymentIntent == null)
                return;

            logger.LogWarning(
                "Payment failed for PaymentIntent {PaymentIntentId}",
                paymentIntent.Id);

           
        }
    }
}
