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
        private readonly CommonResponse _commonResponse;
        public AuthController(IAuthService authService, CommonResponse commonResponse)
        {
            _authService = authService;
            _commonResponse = commonResponse;
        }

        [HttpPost("register-admin")]
        [SwaggerRequestExample(typeof(AdminRegistrationRequestDto), typeof(AdminRegistrationRequestDtoExample))]
        public async Task<IActionResult> CreateAdmin([FromBody] AdminRegistrationRequestDto request)
        {
            var admin = await _authService.RegisterAdminAsync(request);
            var resp = _commonResponse.Created(admin);
            return StatusCode(resp.HttpStatusCode, resp);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-manager")]

        public async Task<IActionResult> CreateManager([FromBody] ManagerRegistrationRequestDto request)
        {
            var manager = await _authService.RegisterManagerAsync(request);
            var resp = _commonResponse.Created(manager);
            return StatusCode(resp.HttpStatusCode, resp);
        }

        [HttpPost("register-guest")]
        [SwaggerRequestExample(typeof(GuestRegistrationRequestDto), typeof(GuestRegistrationRequestDtoExample))]
        public async Task<IActionResult> CreateGuest([FromBody] GuestRegistrationRequestDto request)
        {
            var guest = await _authService.RegisterGuestAsync(request);
            var resp = _commonResponse.Created(guest);
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
            var resp = _commonResponse.Success(response);
            return StatusCode(resp.HttpStatusCode,resp);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string token)
        {
            var response = await _authService.RefreshTokenAsync(token);
            var resp = _commonResponse.Success(response);
            return StatusCode(resp.HttpStatusCode, resp);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
    [FromBody] ForgotPasswordDto model)
        {
            await _authService.ForgotPasswordAsync(model.Email);

            var resp = _commonResponse.Success(null);

            return StatusCode(resp.HttpStatusCode, resp);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
    [FromBody] ResetPasswordDto model)
        {
            await _authService.ResetPasswordAsync(model);

            var resp = _commonResponse.Success(null);

            return StatusCode(resp.HttpStatusCode, resp);
        }
    }
}
