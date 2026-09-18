using HMS.Application.Contracts.Persistance;
using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.AuthDtos;
using HMS.Domain.Entities;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoviesApi.Application.Models.Notification;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HMS.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IConfiguration _configuration;
        private readonly IRedisService _redisService;
        private readonly IManagerService _managerService;
        private readonly IEmailService _emailService;
        private readonly IAdminService _adminService;
        private readonly IGuestService _guestService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private const string _adminRole = "Admin";
        private const string _managerRole = "Manager";
        private const string _guestRole = "Guest";

        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled);

        private static readonly Regex PhoneRegex = new Regex(
            @"^\+995\d{9}$",
            RegexOptions.Compiled);

        private static readonly Regex PersonalNumberRegex = new Regex(
            @"^\d{11}$",
            RegexOptions.Compiled);

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IJwtTokenGenerator jwtTokenGenerator, 
            IConfiguration configuration,
            IRedisService redisService,
            IManagerService managerService,
            IEmailService emailService,
            IAdminService adminService,
            IGuestService guestService,
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _configuration = configuration;
            _redisService = redisService;
            _managerService = managerService;
            _emailService = emailService;
            _adminService = adminService;
            _guestService = guestService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        #region Register Admin
        public async Task<string> RegisterAdminAsync(AdminRegistrationRequestDto model)
        {
            await _unitOfWork.BeginTransactionAsync();
            if (string.IsNullOrEmpty(model.PersonalNumber))
                throw new BadRequestException("Personal number is required");

            if (model.PersonalNumber.Length != 11)
                throw new BadRequestException("Personal number must be 11 characters long");

            if (!PersonalNumberRegex.IsMatch(model.PersonalNumber))
                throw new BadRequestException("Personal number must contain only digits");

            if (string.IsNullOrEmpty(model.Email))
                throw new BadRequestException("Email is required");

            if (!EmailRegex.IsMatch(model.Email))
                throw new BadRequestException("Invalid email format");

            if (string.IsNullOrEmpty(model.PhoneNumber))
                throw new BadRequestException("Phone number is required");

            if (!PhoneRegex.IsMatch(model.PhoneNumber))
                throw new BadRequestException("Phone number must be +995XXXXXXXXX format");

            if (await _userManager.Users.AnyAsync(u => u.PersonalNumber == model.PersonalNumber))  
                throw new BadRequestException("A user with this personal number already exists");
            
            if(await _userManager.Users.AnyAsync(u => u.Email == model.Email))
                throw new BadRequestException("A user with this email already exists");
            
            if(await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber))
                throw new BadRequestException("A user with this phone number already exists");

            try
            {
                var user = _mapper.Map<ApplicationUser>(model);

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    throw new BadRequestException(
           result.Errors.First().Description);

                }

                var admin = new Admin
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    ApplicationUserId = user.Id
                };

                await _adminService.CreateAdminAsync(admin);

                await AddRoleAsync(user, _adminRole);

                await _unitOfWork.CommitTransactionAsync();

                await SendConfirmationCodeAsync(user);

                return user.Id;

            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }



        }
        #endregion

        #region Register Manager
        public async Task<string> RegisterManagerAsync(ManagerRegistrationRequestDto model)
        {
            await _unitOfWork.BeginTransactionAsync();
            if (string.IsNullOrEmpty(model.PersonalNumber))
                throw new BadRequestException("Personal number is required");

            if (model.PersonalNumber.Length != 11)
                throw new BadRequestException("Personal number must be 11 characters long");

            if (!PersonalNumberRegex.IsMatch(model.PersonalNumber))
                throw new BadRequestException("Personal number must contain only digits");

            if (string.IsNullOrEmpty(model.Email))
                throw new BadRequestException("Email is required");

            if (!EmailRegex.IsMatch(model.Email))
                throw new BadRequestException("Invalid email format");

            if (string.IsNullOrEmpty(model.PhoneNumber))
                throw new BadRequestException("Phone number is required");

            if (!PhoneRegex.IsMatch(model.PhoneNumber))
                throw new BadRequestException("Phone number must be 9 characters long");

            if (await _userManager.Users.AnyAsync(u => u.PersonalNumber == model.PersonalNumber))
                throw new BadRequestException("A user with this personal number already exists");

            if (await _userManager.Users.AnyAsync(u => u.Email == model.Email))
                throw new BadRequestException("A user with this email already exists");

            if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber))
                throw new BadRequestException("A user with this phone number already exists");



            try
            {
                var user = _mapper.Map<ApplicationUser>(model);



                var result = await _userManager.CreateAsync(user, model.Password);



                if (!result.Succeeded)
                {
                    throw new BadRequestException(
           result.Errors.First().Description);

                }
                var manager = new Manager
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    HotelId = model.HotelId,
                    ApplicationUserId = user.Id
                };

                await _managerService.CreateManagerAsync(manager);

                await AddRoleAsync(user, _managerRole);

                await _unitOfWork.CommitTransactionAsync();
                await SendConfirmationCodeAsync(user);

                return user.Id;

            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }


        }
        #endregion

        #region Register Guest
        public async Task<string> RegisterGuestAsync(GuestRegistrationRequestDto model)
        {
            await _unitOfWork.BeginTransactionAsync();
            if (string.IsNullOrEmpty(model.PersonalNumber))
                throw new BadRequestException("Personal number is required");

            if (model.PersonalNumber.Length != 11)
                throw new BadRequestException("Personal number must be 11 characters long");

            if (!PersonalNumberRegex.IsMatch(model.PersonalNumber))
                throw new BadRequestException("Personal number must contain only digits");

            if (string.IsNullOrEmpty(model.Email))
                throw new BadRequestException("Email is required");

            if (!EmailRegex.IsMatch(model.Email))
                throw new BadRequestException("Invalid email format");

            if (string.IsNullOrEmpty(model.PhoneNumber))
                throw new BadRequestException("Phone number is required");


            if (!PhoneRegex.IsMatch(model.PhoneNumber))
                throw new BadRequestException("Phone number must be 9 characters long");

            if (await _userManager.Users.AnyAsync(u => u.PersonalNumber == model.PersonalNumber))
                throw new BadRequestException("A user with this personal number already exists");

            if (await _userManager.Users.AnyAsync(u => u.Email == model.Email))
                throw new BadRequestException("A user with this email already exists");

            if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber))
                throw new BadRequestException("A user with this phone number already exists");


            try
            {
                var user = _mapper.Map<ApplicationUser>(model);

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    throw new BadRequestException(
           result.Errors.First().Description);

                }


                var guest = new Guest
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    ApplicationUserId = user.Id
                };

                await _guestService.CreateNewGuestAsync(guest);

                await AddRoleAsync(user, _guestRole);

                await _unitOfWork.CommitTransactionAsync();

                await SendConfirmationCodeAsync(user);

                return user.Id;

            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }


        }
        #endregion

        #region Confirm Email
        public async Task ConfirmEmailAsync(ConfirmEmailDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (user.EmailConfirmed)
                throw new BadRequestException("Email is already confirmed.");

            var key = $"email-confirm:{user.Id}";

            var savedCode = await _redisService.GetAsync(key);

            if (savedCode == null)
                throw new BadRequestException("Verification code has expired.");

            if (savedCode != model.Code)
                throw new BadRequestException("Invalid verification code.");

            user.EmailConfirmed = true;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new Exception("Failed to confirm email.");

            await _redisService.RemoveAsync(key);
        }
        #endregion

        #region Resend Confirmation Email

        public async Task ResendConfirmationCodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (user.EmailConfirmed)
                throw new BadRequestException("Email is already confirmed.");

            var normalizedEmail = email.Trim().ToLower();

            var redisKey = $"confirmation-resend:{normalizedEmail}";

            var cooldown = await _redisService.GetAsync(redisKey);

            if (cooldown != null)
                throw new BadRequestException(
                    "Please wait 60 seconds before requesting a new confirmation code."
                );

            var hourlyKey = $"confirmation-resend-hourly:{normalizedEmail}";

            var hourlyCountString = await _redisService.GetAsync(hourlyKey);

            var hourlyCount = 0;

            if (hourlyCountString != null)
                hourlyCount = int.Parse(hourlyCountString);

            if (hourlyCount >= 5)
                throw new BadRequestException(
                    "You have reached the maximum number of confirmation code requests. Please try again later."
                );

            await SendConfirmationCodeAsync(user);

            await _redisService.SetAsync(
                redisKey,
                "1",
                TimeSpan.FromSeconds(60)
            );

            await _redisService.SetAsync(
            hourlyKey,
            (hourlyCount + 1).ToString(),
            TimeSpan.FromHours(1)
);
        }
        #endregion

        #region Login
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (!user.EmailConfirmed)
                throw new BadRequestException("Email is not confirmed.");

            if (await _userManager.IsLockedOutAsync(user))
                throw new BadRequestException("Your account is locked.");

            bool isValid = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!isValid)
            {
                await _userManager.AccessFailedAsync(user);
                throw new BadRequestException("Username or Password is Incorrect!");
            }
            else
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }

            var roles = await _userManager.GetRolesAsync(user);

            return await GenerateTokenPairAsync(user, roles);
        }
        #endregion


        #region Refresh Token
        public async Task<LoginResponseDto> RefreshTokenAsync(string token)
        {
            var userId = await _redisService.GetAsync(
       $"refresh-token:{token}");

            if (string.IsNullOrEmpty(userId))
                throw new BadRequestException("Invalid or expired refresh token.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException("User not found.");


            await _redisService.RemoveAsync(
                $"refresh-token:{token}");

            var roles = await _userManager.GetRolesAsync(user);




            return await GenerateTokenPairAsync(user, roles);
        }
        #endregion

        #region Forgot Password
        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new NotFoundException("User not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token));

            var resetLink =
                $"https://localhost:7042/api/auth/reset-password" +
                $"?email={Uri.EscapeDataString(user.Email)}" +
                $"&token={Uri.EscapeDataString(encodedToken)}";

            await _emailService.Send(
                user.Email,
                "Reset Password",
                 $"Your Password Reset Token is: " +
                 $"{encodedToken}");
        }
        #endregion


        #region Reset Password
        public async Task ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                throw new NotFoundException("User not found.");

            string token;

            try
            {
                token = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(model.Token));
            }
            catch
            {
                throw new BadRequestException("Invalid reset token.");
            }

            var result = await _userManager.ResetPasswordAsync(
                user,
                token,
                model.NewPassword);

            if (!result.Succeeded)
            {
                throw new BadRequestException(
                    result.Errors.First().Description);
            }
        }
        #endregion

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var storedToken = await _redisService.GetAsync(
                $"refresh-token:{refreshToken}");

            if (string.IsNullOrEmpty(storedToken))
                throw new BadRequestException("Invalid or expired refresh token.");

            await _redisService.RemoveAsync(
                $"refresh-token:{refreshToken}");
        }
        #region Generate Tokens(private)
        private async Task<LoginResponseDto> GenerateTokenPairAsync(ApplicationUser user, IList<string> roles)
        {
            var accessToken = _jwtTokenGenerator.GenerateToken(user, roles);

            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            await _redisService.SetAsync(
                $"refresh-token:{refreshToken}",
                user.Id,
                TimeSpan.FromDays(int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"])));

            return new LoginResponseDto { AccessToken = accessToken, RefreshToken = refreshToken };
        }
        #endregion


        #region Send Confirmation Code(private)
        private async Task SendConfirmationCodeAsync(ApplicationUser user)
        {
            var code = Random.Shared.Next(100000, 999999).ToString();

            await _redisService.SetAsync(
                $"email-confirm:{user.Id}",
                code,
                TimeSpan.FromMinutes(5));
            var baseUrl = _configuration["AppSettings:BaseUrl"];
            var resendCodeLink =
                $"{baseUrl}/api/auth/resend-confirmation-code?email={user.Email}";

            await _emailService.Send(
                user.Email,
                "Email Confirmation",
                $"""
        <h3>Your confirmation code is: <strong>{code}</strong></h3>

        <h4>Didn't receive a code?</h4>

        <a href="{resendCodeLink}"
           style="
               background-color:blue;
               color:white;
               padding:10px 20px;
               text-decoration:none;
               border-radius:5px;
               display:inline-block;">
            Resend Confirmation Code
        </a>
        """);
        }
        #endregion

        #region Add Role(private)
        private async Task AddRoleAsync(
ApplicationUser user,
string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(
                    new IdentityRole(role));
            }

            await _userManager.AddToRoleAsync(user, role);
        }


        #endregion





    }
}
