using HMS.Application.Contracts.Persistance;
using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.Common;
using HMS.Application.Models.GuestDtos;
using HMS.Domain.Entities;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HMS.Application.Service
{
    public class GuestService : IGuestService
    {
        private readonly IGuestRepository _guestRepository;
        private readonly IMapper _mapper;
        private readonly IReservationRepository _reservationRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public GuestService(IGuestRepository guestRepository, IMapper mapper, IReservationRepository reservationRepository,
            UserManager<ApplicationUser> userManager)
        {
            _guestRepository = guestRepository;
            _mapper = mapper;
            _reservationRepository = reservationRepository;
            _userManager = userManager;
        }

        public async Task<GuestForGettingDto> CreateNewGuestAsync(Guest model)
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


            await _guestRepository.AddAsync(model);
            await _guestRepository.SaveAsync();

            return _mapper.Map<GuestForGettingDto>(model);

        }

        public async Task<int> DeleteGuestAsync(int id)
        {
            var guest = await _guestRepository.GetAsync(
                s => s.Id == id,
                include: query => query.Include(x => x.ApplicationUser));

            if (guest == null)
                throw new NotFoundException("Guest Could not be found!");

            var hasActiveOrFutureReservation =
                await _reservationRepository.ExistsAsync(
                    x => x.GuestId == id &&
                         x.CheckOutDate > DateTime.UtcNow);

            if (hasActiveOrFutureReservation)
                throw new BadRequestException(
                    "Guest cannot be deleted because they have an active or future reservation.");

            var applicationUser = guest.ApplicationUser;

            _guestRepository.Remove(guest);

            if (applicationUser != null)
            {
                var result = await _userManager.DeleteAsync(applicationUser);

                if (!result.Succeeded)
                {
                    throw new BadRequestException(
                        result.Errors.First().Description);
                }
            }

            await _guestRepository.SaveAsync();

            return guest.Id;
        }



        public async Task<GuestForGettingDto> UpdateGuestAsync(GuestForUpdatingDto model)
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

            if (string.IsNullOrEmpty(model.PersonalNumber))
                throw new BadRequestException("Personal number is required");

            if (model.PersonalNumber.Length != 11)
                throw new BadRequestException("Personal number must be 11 characters long");

            if (string.IsNullOrEmpty(model.PhoneNumber))
                throw new BadRequestException("Phone number is required");
            if (model.PhoneNumber.Length != 9)
                throw new BadRequestException("Phone number must be 9 characters long");
            var guest = await _guestRepository.GetAsync(s => s.Id == model.Id,
                include: query => query.Include(x => x.ApplicationUser));
            if (guest == null)
                throw new NotFoundException("Guest not found");

            //_mapper.Map(model, guest);
            //_guestRepository.Update(guest);
            guest.FirstName = model.FirstName;
            guest.LastName = model.LastName;
            guest.ApplicationUser.PersonalNumber = model.PersonalNumber;
            guest.ApplicationUser.PhoneNumber = model.PhoneNumber;
            await _guestRepository.SaveAsync();

            return _mapper.Map<GuestForGettingDto>(guest);
        }


       
    }
}
