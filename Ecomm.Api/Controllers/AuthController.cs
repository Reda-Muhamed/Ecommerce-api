using Ecomm.Core.DTOs;
using Ecomm.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ecomm.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly IAuthService userService;
        private readonly IDeviceInfoProvider deviceInfoProvider;
        private readonly ILogger<AuthController> logger;

        public AuthController(IAuthService userService,IDeviceInfoProvider deviceInfoProvider,ILogger<AuthController> logger)
        {
            this.userService = userService;
            this.deviceInfoProvider = deviceInfoProvider;
            this.logger = logger;
        }


        [HttpPost("signup")]
        [ProducesResponseType(typeof(object),(int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(object),(int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto signUpDto , CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .SelectMany(ms => ms.Value.Errors.Select(e => new { Field = ms.Key, Message = e.ErrorMessage }))
                    .ToArray();
                return BadRequest(new {errors});
            }


            try
            {
                DeviceInfoDto deviceInfo = deviceInfoProvider.GetDeviceInfo();

                var result = await userService.CreateUserAsync(signUpDto, deviceInfo,cancellationToken);
                if (!result.IsSuccess)
                {
                    return BadRequest(new { errors = result.Errors.Select(e=>new { Message=e }) });
                }
                return Created();
            }
            catch (OperationCanceledException)
            {
                // Propagate cancellation cleanly
                logger.LogInformation("Signup request was cancelled.");
                return StatusCode((int)HttpStatusCode.RequestTimeout, new { errors = new[] { new { Message = "Request was cancelled." } } });
            }
            catch (Exception ex)
            {
                // Log the exception with details for internal debugging, but return a generic message to client.
                logger.LogError(ex, "Unexpected error during signup for email {Email}", signUpDto?.Email);
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { errors = new[] { new { Message = "An unexpected error occurred. Please try again later." } } });
            }
        }
    }
}
