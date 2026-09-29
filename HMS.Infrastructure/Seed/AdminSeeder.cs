using HMS.Domain.Entities;
using HMS.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Seed
{
    public class AdminSeeder
    {
        private readonly IConfiguration _configuration;


        public AdminSeeder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SeedAsync(
        IServiceProvider serviceProvider)
        {
            var userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            var adminEmail =
                   _configuration["AdminSeed:Email"];

            var adminPassword =
                 _configuration["AdminSeed:Password"];

            var adminPersonalNumber =
                 _configuration["AdminSeed:PersonalNumber"];

            var adminPhoneNumber =
                 _configuration["AdminSeed:PhoneNumber"];

            if (string.IsNullOrWhiteSpace(adminEmail))
                throw new Exception("AdminSeed:Email is not configured.");

            if (string.IsNullOrWhiteSpace(adminPassword))
                throw new Exception("AdminSeed:Password is not configured.");

            if (string.IsNullOrWhiteSpace(adminPersonalNumber))
                throw new Exception("AdminSeed:PersonalNumber is not configured.");

            if (string.IsNullOrWhiteSpace(adminPhoneNumber))
                throw new Exception("AdminSeed:PhoneNumber is not configured.");

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(
                    new IdentityRole("Admin"));
            }

            var existingAdmin =
                await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin != null)
                return;

            if (existingAdmin == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    PersonalNumber = adminPersonalNumber,
                    PhoneNumber = adminPhoneNumber
                };

                var result = await userManager.CreateAsync(
                    admin,
                    adminPassword);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        string.Join(", ", result.Errors.Select(x => x.Description)));
                }

                await userManager.AddToRoleAsync(admin, "Admin");
            }

            var existingAdminProfile = await context.Admins.FirstOrDefaultAsync(a => a.ApplicationUserId == existingAdmin.Id);

            if (existingAdminProfile == null)
            {
                var adminProfile = new Admin
                {
                    ApplicationUserId = existingAdmin.Id,
                    FirstName = _configuration["AdminSeed:FirstName"],
                    LastName = _configuration["AdminSeed:LastName"]
                };
               await context.Admins.AddAsync(adminProfile);
                await context.SaveChangesAsync();

            }
        }
    }
}
