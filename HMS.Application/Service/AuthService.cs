using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.AuthDtos;
using HMS.Domain.Entities;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MoviesApi.Application.Models.Notification;
using System;
using System.Collections.Generic;
using System.Text;

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
        private readonly IMapper _mapper;

        private const string _adminRole = "Admin";
        private const string _managerRole = "Manager";

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IJwtTokenGenerator jwtTokenGenerator, 
            IConfiguration configuration,
            IRedisService redisService,
            IManagerService managerService,
            IEmailService emailService,
            IAdminService adminService,
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
            _mapper = mapper;
        }


        public async Task<string> RegisterAdminAsync(AdminRegistrationRequestDto model)
        {
            var user = _mapper.Map<ApplicationUser>(model);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var userToReturn = await _userManager.FindByEmailAsync(model.Email);
                if (userToReturn != null)
                {
                    if (!await _roleManager.RoleExistsAsync(_adminRole))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(_adminRole));
                    }
                    await _userManager.AddToRoleAsync(userToReturn, _adminRole);
                    var code = Random.Shared.Next(100000, 999999).ToString();
                    await _redisService.SetAsync(
                        $"email-confirm:{user.Id}",
                        code,
                        TimeSpan.FromMinutes(5));
                    var resendCodeLink = $"https://localhost:7042/api/auth/resend-confirmation-code?email={model.Email}";
                    await _emailService.Send(
                        model.Email,
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
                            """
                        );
                }

                var admin = new Admin
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PersonalNumber = model.PersonalNumber,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    ApplicationUserId = user.Id
                };

                await _adminService.CreateAdminAsync(admin);



                return userToReturn.Id;

            }
            else
            {
                throw new BadRequestException(result.Errors.FirstOrDefault().Description);
            }
        }
        public async Task<string> RegisterManagerAsync(ManagerRegistrationRequestDto model)
        {
            var user = _mapper.Map<ApplicationUser>(model);



            var result = await _userManager.CreateAsync(user, model.Password);

            

            if (result.Succeeded)
            {
                var userToReturn = await _userManager.FindByEmailAsync(model.Email);

                if(userToReturn != null)
                {
                    if (!await _roleManager.RoleExistsAsync(_managerRole)) 
                    {
                        await _roleManager.CreateAsync(new IdentityRole(_managerRole));
                    }

                    await _userManager.AddToRoleAsync(userToReturn, _managerRole);
                    var code = Random.Shared.Next(100000, 999999).ToString();

                    await _redisService.SetAsync(
                        $"email-confirm:{user.Id}",
                        code,
                        TimeSpan.FromMinutes(5));

                    var resendCodeLink = $"https://localhost:7042/api/auth/resend-confirmation-code?email={model.Email}";

                    await _emailService.Send(
                        model.Email,
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
                            """
                        );

                }

                var manager = new Manager
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PersonalNumber = model.PersonalNumber,
                    HotelId = model.HotelId,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    ApplicationUserId = user.Id
                };

                await _managerService.CreateManagerAsync(manager);

                return userToReturn.Id;
            }
            else
            {
                throw new BadRequestException(result.Errors.FirstOrDefault().Description);
            }
        }

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

        public async Task ResendConfirmationCodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (user.EmailConfirmed)
                throw new BadRequestException("Email is already confirmed.");

            var code = Random.Shared.Next(100000, 999999).ToString();

            await _redisService.SetAsync(
                $"email-confirm:{user.Id}",
                code,
                TimeSpan.FromMinutes(5));

            var resendCodeLink = $"https://localhost:7042/api/auth/resend-confirmation-code?email={email}";

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
                            """
                );
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (!user.EmailConfirmed)
                throw new BadRequestException("Email is not confirmed.");

            if(await _userManager.IsLockedOutAsync(user))
                throw new BadRequestException("Your account is locked.");

            bool isValid = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!isValid) throw new BadRequestException("Username or Password is Incorrect!");

            var roles = await _userManager.GetRolesAsync(user);

            return await GenerateTokenPairAsync(user, roles);
        }


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
            var resp = await GenerateTokenPairAsync(user, roles);

            await _redisService.SetAsync(
        $"refresh-token:{resp.RefreshToken  }",
        user.Id,
        TimeSpan.FromDays(
            int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"])));

            return resp;
        }


    }
}
