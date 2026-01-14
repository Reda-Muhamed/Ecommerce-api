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

                var result = await userService.CreateUserAsync(signUpDto,cancellationToken);
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

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(
                [FromBody] ConfirmEmailDto dto,
                CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await userService.ConfirmEmailAsync(dto.UserId, dto.Token, ct);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Email confirmed successfully" });
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto signInDto, CancellationToken cancellationToken)
        {
            var deviceInfo = deviceInfoProvider.GetDeviceInfo();

            var result = await userService.SignInAsync(signInDto, deviceInfo, cancellationToken);

            if (!result.IsSuccess)
            {
                logger.LogWarning("Failed login attempt for email {Email} from IP {IP}",
                    signInDto.Email,
                    deviceInfo.IpAddress);

                return Unauthorized(new { errors = result.Errors });
            }

            logger.LogInformation("User {Email} logged in successfully from IP {IP}",
                signInDto.Email,
                deviceInfo.IpAddress);

            return Ok(result.Value);
        }



    }
}
