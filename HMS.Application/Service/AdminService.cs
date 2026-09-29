using HMS.Application.Contracts.Persistance;
using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.AuthDtos;
using HMS.Domain.Entities;
using MapsterMapper;
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
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(IAdminRepository adminRepository, IApplicationUserRepository applicationUserRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper, IUnitOfWork unitOfWork)
        {
            _adminRepository = adminRepository;
            _applicationUserRepository = applicationUserRepository;
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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

        public async Task<string> DeleteAdminAsync(string userId)
        {

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var admin = await _adminRepository.GetAsync(
                x => x.ApplicationUserId == userId,
                include: query => query.Include(x => x.ApplicationUser)
            );

                if (admin == null)
                    throw new NotFoundException("Admin not found!");

                var hasAnotherAdmin = await _adminRepository.ExistsAsync(
                    x => x.Id != admin.Id
                );

                if (!hasAnotherAdmin)
                    throw new BadRequestException(
                        "Admin cannot be deleted because there must be at least one Admin."
                    );

                var applicationUser = admin.ApplicationUser;

                _adminRepository.Remove(admin);

                if (applicationUser != null)
                {
                    var result = await _userManager.DeleteAsync(applicationUser);

                    if (!result.Succeeded)
                    {
                        throw new BadRequestException(
                            result.Errors.First().Description
                        );
                    }
                }

               
                await _adminRepository.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                return userId;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
            
        }
        #region Get All Admin
        public async Task<IEnumerable<AdminForGettingDto>> GetAllAdminAsync()
        {
            var admins = await _adminRepository.GetAllAsync(
                includes: q => q.Include(a => a.ApplicationUser));

            return _mapper.Map<IEnumerable<AdminForGettingDto>>(admins.Items);


        }
        #endregion

    }
}
