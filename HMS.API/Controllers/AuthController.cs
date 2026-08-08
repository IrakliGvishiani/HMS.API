using HMS.Application.Contracts.Service;
using HMS.Application.Models.AuthDtos;
using HMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Application.Models.Notification;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using static CommonResponse;
using static HMS.API.Examples;

namespace HMS.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register-admin")]
        [SwaggerRequestExample(typeof(AdminRegistrationRequestDto), typeof(AdminRegistrationRequestDtoExample))]
        public async Task<IActionResult> CreateAdmin([FromBody] AdminRegistrationRequestDto request)
        {
            var admin = await _authService.RegisterAdminAsync(request);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, admin, true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(resp.HttpStatusCode, resp);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-manager")]

        public async Task<IActionResult> CreateManager([FromBody] ManagerRegistrationRequestDto request)
        {
            var manager = await _authService.RegisterManagerAsync(request);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, manager, true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(resp.HttpStatusCode, resp);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto model)
        {
            await _authService.ConfirmEmailAsync(model);

            return Ok(new CommonResponse(CommonResponseMessage.SuccessMessage, null, true, Convert.ToInt32(HttpStatusCode.OK)));
        }

        [HttpGet("resend-confirmation-code")]
        public async Task<IActionResult> ResendConfirmationCode(string email)
        {
            await _authService.ResendConfirmationCodeAsync(email);

            return Ok(new CommonResponse(CommonResponseMessage.SuccessMessage, null, true, Convert.ToInt32(HttpStatusCode.OK)));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var response = await _authService.LoginAsync(model);
            return Ok(new CommonResponse(CommonResponseMessage.SuccessMessage, response, true, Convert.ToInt32(HttpStatusCode.OK)));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string token)
        {
            var response = await _authService.RefreshTokenAsync(token);
            return Ok(new CommonResponse(CommonResponseMessage.SuccessMessage, response, true, Convert.ToInt32(HttpStatusCode.OK)));
        }
    }
}
