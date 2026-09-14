
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
            return this.ToActionResult(_commonResponse.Success(admin));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-manager")]

        public async Task<IActionResult> CreateManager([FromBody] ManagerRegistrationRequestDto request)
        {
            var manager = await _authService.RegisterManagerAsync(request);
            return this.ToActionResult(_commonResponse.Created(manager));
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

            return this.ToActionResult(_commonResponse.Success(null));
        }

        [HttpGet("resend-confirmation-code")]
        public async Task<IActionResult> ResendConfirmationCode(string email)
        {
            await _authService.ResendConfirmationCodeAsync(email);

            return this.ToActionResult(_commonResponse.Success(null));
        }

        [HttpPost("login")]
        [SwaggerRequestExample(typeof(LoginRequestDto),typeof(LoginRequestDtoExample))]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var response = await _authService.LoginAsync(model);
            return this.ToActionResult(_commonResponse.Success(response));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string token)
        {
            var response = await _authService.RefreshTokenAsync(token);
            return this.ToActionResult(_commonResponse.Success(response));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
    [FromBody] ForgotPasswordDto model)
        {
            await _authService.ForgotPasswordAsync(model.Email);

            return this.ToActionResult(_commonResponse.Success(null));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
    [FromBody] ResetPasswordDto model)
        {
            await _authService.ResetPasswordAsync(model);

            return this.ToActionResult(_commonResponse.Success(null));
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] string refreshToken)
        {
            await _authService.RevokeTokenAsync(refreshToken);

            return this.ToActionResult(_commonResponse.Success(null));
        }
    }
}
