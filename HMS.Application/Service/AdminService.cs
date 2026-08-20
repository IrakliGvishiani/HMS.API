using HMS.Application.Contracts.Persistance;
using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HMS.Application.Service
{
    public class AdminService : IAdminService
    {

        private readonly IAdminRepository _adminRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(IAdminRepository adminRepository, IApplicationUserRepository applicationUserRepository,
            UserManager<ApplicationUser> userManager)
        {
            _adminRepository = adminRepository;
            _applicationUserRepository = applicationUserRepository;
            _userManager = userManager;
        }


        public async Task<int> CreateAdminAsync(Admin model)
        {
            if (model == null) throw new BadRequestException("Model is required");

            if (string.IsNullOrEmpty(model.FirstName))
                throw new BadRequestException("First name is required");

            if (model.FirstName.Length < 2 || model.FirstName.Length > 100)
                throw new BadRequestException("First name must be between 2 and 100 characters");

            if (string.IsNullOrEmpty(model.LastName))
                throw new BadRequestException("Last name is required");

            if (model.LastName.Length < 2 || model.LastName.Length > 100)
                throw new BadRequestException("Last name must be between 2 and 100 characters");



            await _adminRepository.AddAsync(model);
            
            return model.Id;
        }

        public async Task<int> DeleteAdminAsync(int id,string userId)
        {

            var admin = await _adminRepository.GetAsync(x => x.Id == id,
                include: query => query.Include(x => x.ApplicationUser));

            if (admin == null) throw new NotFoundException("Admin not found!");
         
            var adminUser = await _userManager.FindByIdAsync(userId);

            if (adminUser == null) throw new NotFoundException("User Not Found!");

            var hasAnotherAdmin = await _adminRepository.ExistsAsync(
                x => x.Id != admin.Id
                );

            if (!hasAnotherAdmin)
                throw new BadRequestException("Admin cannot be deleted because this hotel must have at least one Admin");

            if (admin.ApplicationUserId != adminUser.Id)
                throw new BadRequestException("You Can't Delete Another Admin");

            var applicationUser = admin.ApplicationUser;

            _adminRepository.Remove(admin);

            if(applicationUser != null)
            {
               var result = await _userManager.DeleteAsync(applicationUser);

                if (!result.Succeeded)
                {
                    throw new BadRequestException(
                       result.Errors.First().Description);
                }
            }

            await _adminRepository.SaveAsync();
            return id;
        }
    }
}
