using HMS.Application.Models.AuthDtos;
using MoviesApi.Application.Models.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Service
{
    public interface IAuthService
    {
        Task<string> RegisterManagerAsync(ManagerRegistrationRequestDto model);

        Task<string> RegisterAdminAsync(AdminRegistrationRequestDto model);
        Task ResendConfirmationCodeAsync(string email);
        Task ConfirmEmailAsync(ConfirmEmailDto model);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto model);

        Task<LoginResponseDto> RefreshTokenAsync(string token);
    }
}
