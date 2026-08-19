using HMS.Application.Contracts.Persistance;
using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.ReservationDtos;
using HMS.Domain.Entities;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HMS.Application.Service
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IGuestRepository _guestRepository;
        private readonly IReservationRoomRepository _reservationRoomRepository;
        private readonly IMapper _mapper;

        public ReservationService(IReservationRepository reservationRepository,IRoomRepository roomRepository,IGuestRepository guestRepository,
            IMapper mapper,
            IReservationRoomRepository reservationRoomRepository)
        {
            _reservationRepository = reservationRepository;
            _roomRepository = roomRepository;
            _guestRepository = guestRepository;
            _mapper = mapper;
            _reservationRoomRepository = reservationRoomRepository;
        }
        public async Task<ReservationForGettingDto> CreateReservationAsync(
    ReservationForCreatingDto model,
    string userId)
        {
            if (model.CheckInDate.Date < DateTime.UtcNow.Date)
                throw new BadRequestException(
                    "Check-in date cannot be before today.");

            if (model.CheckOutDate <= model.CheckInDate)
                throw new BadRequestException(
                    "Check-out date must be after check-in date.");

            if (model.RoomIds == null || !model.RoomIds.Any())
                throw new BadRequestException(
                    "At least one room must be selected.");

            var guest = await _guestRepository.GetAsync(
                x => x.ApplicationUserId == userId);

            if (guest == null)
                throw new NotFoundException("Guest not found.");

            var rooms = await _roomRepository.GetAllAsync(
                filter: room =>
                    model.RoomIds.Contains(room.Id) &&
                    !room.ReservationRooms.Any(rr =>
                        rr.Reservation.CheckInDate < model.CheckOutDate &&
                        rr.Reservation.CheckOutDate > model.CheckInDate),
                tracking: true);

            var availableRooms = rooms.Items.ToList();

            if (availableRooms.Count != model.RoomIds.Count)
            {
                throw new BadRequestException(
                    "One or more selected rooms are not available for the specified dates.");
            }

            var reservation = new Reservation
            {
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                GuestId = guest.Id,
                ReservationRooms = new List<ReservationRoom>()
            };

            foreach (var room in availableRooms)
            {
                reservation.ReservationRooms.Add(
                    new ReservationRoom
                    {
                        RoomId = room.Id,
                        Reservation = reservation
                    });
            }

            await _reservationRepository.AddAsync(reservation);

            await _reservationRepository.SaveAsync();

            return new ReservationForGettingDto
            {
                Id = reservation.Id,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                GuestId= reservation.GuestId,
                

            };
            
        }

        public async Task<int> DeleteReservationAsync(int id, string userId)
        {
            var guest = await _guestRepository.GetAsync(
                x => x.ApplicationUserId == userId);

            if (guest == null)
                throw new NotFoundException("Guest not found.");

            var reservation = await _reservationRepository.GetAsync(
                x => x.Id == id);

            if (reservation == null)
                throw new NotFoundException("Reservation not found.");

            if (reservation.GuestId != guest.Id)
                throw new BadRequestException(
                    "You cannot delete another guest's reservation.");

            _reservationRepository.Remove(reservation);

            await _reservationRepository.SaveAsync();

            return id;
        }

        public async Task<IEnumerable<ReservationForGettingDto>>
     SearchReservationsAsync(ReservationSearchDto model)
        {
            var today = DateTime.UtcNow;

            var (reservations, _) = await _reservationRepository.GetAllAsync(
                filter: reservation =>
                    (!model.GuestId.HasValue ||
                    reservation.GuestId == model.GuestId.Value)

                    &&

                    (!model.RoomId.HasValue ||
                     reservation.ReservationRooms.Any(
                         rr => rr.RoomId == model.RoomId.Value))

                    &&

                    (!model.HotelId.HasValue ||
                     reservation.ReservationRooms.Any(
                         rr => rr.Room.HotelId == model.HotelId.Value))

                    &&

                    (!model.Date.HasValue ||
                     (
                         reservation.CheckInDate <= model.Date.Value &&
                         reservation.CheckOutDate > model.Date.Value
                     ))

                    &&

                    (!model.Active.HasValue ||
                     (
                         model.Active.Value
                             ? reservation.CheckInDate <= today &&
                               reservation.CheckOutDate > today
                             : reservation.CheckOutDate <= today
                     )),

                tracking: false,

                includes: new Expression<Func<Reservation, object>>[]
                {
            x => x.ReservationRooms
                });

            return _mapper.Map<IEnumerable<ReservationForGettingDto>>(
                reservations);
        }

        public async Task<int> UpdateReservationAsync(ReservationForUpdatingDto model)
        {
            if (model.CheckInDate.Date < DateTime.UtcNow.Date)
                throw new BadRequestException(
                    "Check-in date cannot be before today.");

            if (model.CheckOutDate <= model.CheckInDate)
                throw new BadRequestException(
                    "Check-out date must be after check-in date.");

            var reservation = await _reservationRepository.GetAsync(
                x => x.Id == model.Id,
                include: query => query.Include(x => x.ReservationRooms)
                );

            if (reservation == null) throw new NotFoundException("Reservation Not Found!");

            var roomIds = reservation.ReservationRooms.Select(x => x.RoomId).ToList();


            var hasConflict = await _reservationRoomRepository.ExistsAsync(
                rr => roomIds.Contains(rr.RoomId) &&
                rr.ReservationId != model.Id &&
                rr.Reservation.CheckInDate < model.CheckOutDate &&
                rr.Reservation.CheckOutDate > model.CheckInDate
                );

            if (hasConflict) throw new BadRequestException("One or more rooms are not available for the selected dates.");

            reservation.CheckInDate = model.CheckInDate;
            reservation.CheckOutDate = model.CheckOutDate;

            await _reservationRepository.SaveAsync();

            return reservation.Id;
            
        }
    }
}
