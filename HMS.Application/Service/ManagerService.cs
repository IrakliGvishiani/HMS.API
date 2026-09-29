using HMS.Application.Contracts.Persistance;
using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.Analytics;
using HMS.Application.Models.AuthDtos;
using HMS.Application.Models.ManagerDtos;
using HMS.Domain.Entities;
using HMS.Domain.Enum;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace HMS.Application.Service
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository managerRepository;
        private readonly IHotelService hotelService;
        private readonly IRoomRepository roomRepository;
        private readonly IReservationRoomRepository reservationRoomRepository;
        private readonly IReservationRepository reservationRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        private static readonly Regex PhoneRegex = new Regex(
            @"^\+995\d{9}$",
            RegexOptions.Compiled);

        private static readonly Regex PersonalNumberRegex = new Regex(
            @"^\d{11}$",
            RegexOptions.Compiled);

        public ManagerService(IManagerRepository managerRepository, IHotelService hotelService, IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IRoomRepository roomRepository,
            IReservationRoomRepository reservationRoomRepository,
            IReservationRepository reservationRepository)
        {
            this.managerRepository = managerRepository;
            this.hotelService = hotelService;
            this.mapper = mapper;
            this.userManager = userManager;
            this.roomRepository = roomRepository;
            this.reservationRoomRepository = reservationRoomRepository;
            this.reservationRepository = reservationRepository;
        }
        public async Task<int> CreateManagerAsync(Manager model)
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


            if (model.HotelId <= 0)
                throw new BadRequestException("Invalid hotel ID");

            var hotel = await hotelService.GetHotelAsync(model.HotelId);

            if(hotel == null)
                throw new NotFoundException("Hotel not found");

            await managerRepository.AddAsync(model);
          await managerRepository.SaveAsync();
            return model.HotelId;
        }

        public async Task<int> DeleteManagerAsync(int id)
        {
            var manager = await managerRepository.GetAsync(s => s.Id == id,
                include: query => query.Include(x => x.ApplicationUser));
            if (manager == null)
                throw new NotFoundException("Manager not found");

            var hasAnotherManager = await managerRepository.ExistsAsync(
       x => x.HotelId == manager.HotelId &&
            x.Id != manager.Id);

            if (!hasAnotherManager)
                throw new BadRequestException(
                    "Manager cannot be deleted because this hotel must have at least one manager.");

            var applicationUser = manager.ApplicationUser;

            managerRepository.Remove(manager);

            if(applicationUser != null)
            {
               var result = await userManager.DeleteAsync(applicationUser);

                if (!result.Succeeded)
                {
                    throw new BadRequestException(
                        result.Errors.First().Description);
                }
            }
            await managerRepository.SaveAsync();
            return id;
        }

        public async Task<IEnumerable<ManagerListForGettingDto>> GetManagersAsync()
        {
            
            var managers = await managerRepository.GetAllAsync(
                includes: new Func<IQueryable<Manager>, IQueryable<Manager>>[]
                {
                    m => m.Include(mm => mm.ApplicationUser),
                    m => m.Include(mm => mm.Hotel)
                }
            );

            var managerDtos = mapper.Map<IEnumerable<ManagerListForGettingDto>>(managers.Items);
            return managerDtos;
        }

        public async Task<int> UpdateManagerAsync(ManagerForUpdatingDto model)
        {

            var manager = await managerRepository.GetAsync(s => s.Id == model.Id,
                include: query => query.Include(x => x.ApplicationUser));

            if (manager == null)
                throw new NotFoundException("Manager not found");

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

            if (!PersonalNumberRegex.IsMatch(model.PersonalNumber))
                throw new BadRequestException("Personal number must contain only digits!");

            if (string.IsNullOrEmpty(model.PhoneNumber))
                throw new BadRequestException("Phone number is required");

            if (!PhoneRegex.IsMatch(model.PhoneNumber))
                throw new BadRequestException("Phone number must be in format +995XXXXXXXXX");

          
            manager.FirstName = model.FirstName;
            manager.LastName = model.LastName;
            manager.ApplicationUser.PersonalNumber = model.PersonalNumber;
            manager.ApplicationUser.PhoneNumber = model.PhoneNumber;

            await managerRepository.SaveAsync();
            return manager.Id;
        }

        public async Task<HotelAnalyticsDto> GetManagerAnalyticsAsync(
    string userId,
    string userRole)
        {
            int? hotelId = null;

            if (userRole == "Manager")
            {
                var manager = await managerRepository.GetAsync(
                    x => x.ApplicationUserId == userId);

                if (manager == null)
                    throw new NotFoundException("Manager not found!");

                hotelId = manager.HotelId;
            }
            else if (userRole != "Admin")
            {
                throw new NotAllowedException(
                    "You are not allowed to view analytics.");
            }

            var now = DateTime.UtcNow;

            // -------------------------
            // TOTAL ROOMS
            // -------------------------

            var totalRooms = await roomRepository.CountAsync(
                x => !hotelId.HasValue || x.HotelId == hotelId.Value
            );

            // -------------------------
            // CURRENTLY OCCUPIED
            // -------------------------

            var (occupiedReservationRooms, _) =
                await reservationRoomRepository.GetAllAsync(
                    filter: x =>
                        (!hotelId.HasValue ||
                         x.Room.HotelId == hotelId.Value)
                        &&
                        x.Reservation.Status != ReservationStatus.Cancelled
                        &&
                        x.Reservation.CheckInDate <= now
                        &&
                        x.Reservation.CheckOutDate > now,
                    tracking: false
                );

            var occupiedRooms = occupiedReservationRooms
                .Select(x => x.RoomId)
                .Distinct()
                .Count();

            // -------------------------
            // FUTURE RESERVED
            // -------------------------

            var (reservedReservationRooms, _) =
                await reservationRoomRepository.GetAllAsync(
                    filter: x =>
                        (!hotelId.HasValue ||
                         x.Room.HotelId == hotelId.Value)
                        &&
                        x.Reservation.Status != ReservationStatus.Cancelled
                        &&
                        x.Reservation.CheckInDate > now,
                    tracking: false
                );

            var reservedRooms = reservedReservationRooms
                .Select(x => x.RoomId)
                .Distinct()
                .Count();

            // -------------------------
            // AVAILABLE
            // -------------------------

            var availableRooms =
                totalRooms - occupiedRooms - reservedRooms;

            // -------------------------
            // TOTAL RESERVATIONS
            // -------------------------

            var totalReservations =
                await reservationRepository.CountAsync(
                    x =>
                        !hotelId.HasValue ||
                        x.ReservationRooms.Any(
                            rr => rr.Room.HotelId == hotelId.Value)
                );

            // -------------------------
            // ACTIVE
            // -------------------------

            var activeReservations =
                await reservationRepository.CountAsync(
                    x =>
                        (!hotelId.HasValue ||
                         x.ReservationRooms.Any(
                             rr => rr.Room.HotelId == hotelId.Value))
                        &&
                        x.Status == ReservationStatus.Active
                );

            // -------------------------
            // COMPLETED
            // -------------------------

            var completedReservations =
                await reservationRepository.CountAsync(
                    x =>
                        (!hotelId.HasValue ||
                         x.ReservationRooms.Any(
                             rr => rr.Room.HotelId == hotelId.Value))
                        &&
                        x.Status == ReservationStatus.Completed
                );

            // -------------------------
            // CANCELLED
            // -------------------------

            var cancelledReservations =
                await reservationRepository.CountAsync(
                    x =>
                        (!hotelId.HasValue ||
                         x.ReservationRooms.Any(
                             rr => rr.Room.HotelId == hotelId.Value))
                        &&
                        x.Status == ReservationStatus.Cancelled
                );

            // -------------------------
            // TOTAL GUESTS
            // -------------------------

            var (reservations, _) =
                await reservationRepository.GetAllAsync(
                    filter: x =>
                        !hotelId.HasValue ||
                        x.ReservationRooms.Any(
                            rr => rr.Room.HotelId == hotelId.Value),
                    tracking: false
                );

            var totalGuests = reservations
                .Select(x => x.GuestId)
                .Distinct()
                .Count();

            // -------------------------
            // TOTAL REVENUE
            // -------------------------

            var totalRevenue = 0.0;

            
            if (userRole == "Manager")
            {
                var (reservationRooms, _) =
    await reservationRoomRepository.GetAllAsync(
        filter: x => x.Room.HotelId == hotelId.Value &&
                x.Reservation.Status != ReservationStatus.Cancelled,
        tracking: false,
        includes: new Func<IQueryable<ReservationRoom>, IQueryable<ReservationRoom>>[]
        {
            rr => rr.Include(r => r.Reservation),
            rr => rr.Include(r => r.Room)   
        }
    );

                totalRevenue = reservationRooms.Sum(x =>
                {
                    var nights =
                        (x.Reservation.CheckOutDate -
                         x.Reservation.CheckInDate).Days;

                    return x.Room.Price * nights;
                });
            }
            

            return new HotelAnalyticsDto
            {
                HotelId = hotelId,

                TotalRooms = totalRooms,
                AvailableRooms = availableRooms,
                ReservedRooms = reservedRooms,
                OccupiedRooms = occupiedRooms,

                TotalReservations = totalReservations,
                ActiveReservations = activeReservations,
                CompletedReservations = completedReservations,
                CancelledReservations = cancelledReservations,

                TotalGuests = totalGuests,

                TotalRevenue = totalRevenue
            };
        }

        public async Task<ManagerProfileDto> GetOwnProfileAsync(string userId)
        {
            var manager = await managerRepository.GetAsync(
                x => x.ApplicationUserId == userId,
                include: query => query
                    .Include(x => x.ApplicationUser)
                    .Include(x => x.Hotel));

            if (manager == null)
                throw new NotFoundException("Manager profile not found.");

            return mapper.Map<ManagerProfileDto>(manager);
        }
    }
}
