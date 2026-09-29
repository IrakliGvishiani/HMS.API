using HMS.Application.Contracts.Persistance;
using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.ReservationDtos;
using HMS.Domain.Entities;
using HMS.Domain.Enum;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HMS.Application.Service;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IReservationRoomRepository _reservationRoomRepository;
    private readonly IManagerRepository _managerRepository;
    private readonly IMapper _mapper;

    public ReservationService(
        IReservationRepository reservationRepository,
        IRoomRepository roomRepository,
        IGuestRepository guestRepository,
        IMapper mapper,
        IReservationRoomRepository reservationRoomRepository,
        IManagerRepository managerRepository)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
        _guestRepository = guestRepository;
        _mapper = mapper;
        _reservationRoomRepository = reservationRoomRepository;
        _managerRepository = managerRepository;
    }




    public async Task<ReservationForGettingDto> CreateReservationAsync(
        ReservationForCreatingDto model,
        string userId,
        string userRole)
    {
        if (model == null)
            throw new BadRequestException("Request model is required.");

        if (model.CheckInDate.Date < DateTime.UtcNow.Date)
            throw new BadRequestException(
                "Check-in date cannot be before today.");

        if (model.CheckOutDate <= model.CheckInDate)
            throw new BadRequestException(
                "Check-out date must be after check-in date.");

        if (model.RoomIds == null || !model.RoomIds.Any())
            throw new BadRequestException(
                "At least one room must be selected.");


        int targetGuestId;

        if (userRole == "Guest")
        {
            var guest = await _guestRepository.GetAsync(
                x => x.ApplicationUserId == userId);

            if (guest == null)
                throw new NotFoundException(
                    "Guest profile not found.");

            targetGuestId = guest.Id;
        }
        else
        {
            if (!model.GuestId.HasValue || model.GuestId.Value <= 0)
                throw new BadRequestException(
                    "GuestId is required when reservation is created by Admin or Manager.");

            var guestExists = await _guestRepository.ExistsAsync(
                x => x.Id == model.GuestId.Value);

            if (!guestExists)
                throw new NotFoundException(
                    "Specified Guest not found.");

            targetGuestId = model.GuestId.Value;
        }

        var managerHotelId = await GetManagerHotelIdAsync(
                    userId,
                    userRole);



        var rooms = await _roomRepository.GetAllAsync(
    filter: room =>
        model.RoomIds.Contains(room.Id) &&


        (!managerHotelId.HasValue ||
         room.HotelId == managerHotelId.Value) &&

        !room.ReservationRooms.Any(rr =>
            rr.Reservation.Status != ReservationStatus.Cancelled &&
            rr.Reservation.CheckInDate < model.CheckOutDate &&
            rr.Reservation.CheckOutDate > model.CheckInDate),

    tracking: true
);

        var availableRooms = rooms.Items.ToList();


        if (availableRooms.Count != model.RoomIds.Distinct().Count())
        {
            throw new BadRequestException(
                "One or more selected rooms are not available for the specified dates.");
        }


        var reservation = new Reservation
        {
            CheckInDate = model.CheckInDate,
            CheckOutDate = model.CheckOutDate,
            GuestId = targetGuestId,
            Status = ReservationStatus.Reserved,
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
            GuestId = reservation.GuestId
        };
    }



    public async Task<int> DeleteReservationAsync(
        int id,
        string userId,
        string userRole)
    {
        var reservation = await _reservationRepository.GetAsync(
            x => x.Id == id,
            include: query => query
            .Include(r => r.ReservationRooms)
            .ThenInclude(rr => rr.Room));

        if (reservation == null)
            throw new NotFoundException(
                "Reservation not found.");

        if (userRole == "Manager")
        {
            var managerHotelId = await GetManagerHotelIdAsync(
                userId,
                userRole);

            var belongsToManagerHotel =
                reservation.ReservationRooms.Any(
                    rr => rr.Room.HotelId == managerHotelId);

            if (!belongsToManagerHotel)
            {
                throw new NotAllowedException(
                    "You cannot cancel a reservation for another hotel.");
            }
        }

       
        if (userRole == "Guest")
        {
            var guest = await _guestRepository.GetAsync(
                x => x.ApplicationUserId == userId);

            if (guest == null)
                throw new NotFoundException(
                    "Guest profile not found.");

            if (reservation.GuestId != guest.Id)
            {
                throw new NotAllowedException(
                    "You cannot cancel another guest's reservation.");
            }
        }


        reservation.Status = ReservationStatus.Cancelled;

        _reservationRepository.Update(reservation);

        await _reservationRepository.SaveAsync();

        return reservation.Id;
    }




    public async Task<IEnumerable<ReservationForGettingDto>> SearchReservationsAsync(
        ReservationSearchDto model,
        string userId,
        string userRole)
    {
        if (model == null)
            throw new BadRequestException(
                "Request model is required.");


        var today = DateTime.UtcNow;

        int? filteredGuestId = model.GuestId;

        var managerHotelId = await GetManagerHotelIdAsync(
                userId,
                userRole);

        
        if (userRole == "Guest")
        {
            var guest = await _guestRepository.GetAsync(
                x => x.ApplicationUserId == userId);

            if (guest == null)
                throw new NotFoundException(
                    "Guest profile not found.");

            filteredGuestId = guest.Id;
        }


        var (reservations, _) =
    await _reservationRepository.GetAllAsync(
        filter: reservation =>
            (
                !filteredGuestId.HasValue ||
                reservation.GuestId == filteredGuestId.Value
            )
            &&
            (
                !model.RoomId.HasValue ||
                reservation.ReservationRooms.Any(rr => rr.RoomId == model.RoomId.Value)
            )
            &&
            (
                !model.HotelId.HasValue ||
                reservation.ReservationRooms.Any(rr => rr.Room.HotelId == model.HotelId.Value)
            )
            &&
            (
                !model.Date.HasValue ||
                (
                    reservation.CheckInDate <= model.Date.Value &&
                    reservation.CheckOutDate > model.Date.Value
                )
            )
            &&
            (
                !model.Active.HasValue ||
                (
                    model.Active.Value
                        ? reservation.CheckInDate <= today &&
                          reservation.CheckOutDate > today &&
                          reservation.Status != ReservationStatus.Cancelled
                        : reservation.CheckOutDate <= today
                )
            )
            &&
            (
                !managerHotelId.HasValue ||
                reservation.ReservationRooms.Any(rr => rr.Room.HotelId == managerHotelId.Value)
            ),
        tracking: false,
        includes: new Func<IQueryable<Reservation>, IQueryable<Reservation>>[]
        {
            query => query.Include(x => x.Guest).ThenInclude(x => x.ApplicationUser),
            query => query.Include(x => x.ReservationRooms).ThenInclude(x => x.Room).ThenInclude(x => x.Hotel)
        }
    );


        return _mapper.Map<List<ReservationForGettingDto>>(
            reservations);
    }

    public async Task<ReservationForGettingDto> GetReservationByIdAsync(
    int id,
    string userId,
    string userRole)
    {
        var reservation = await _reservationRepository.GetAsync(
            x => x.Id == id,
            include: query => query
                .Include(r => r.Guest)
                .ThenInclude(g => g.ApplicationUser)
                .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Room)
                .ThenInclude(room => room.Hotel));

        if (reservation == null)
            throw new NotFoundException("Reservation not found.");

        if (userRole == "Guest")
        {
            var guest = await _guestRepository.GetAsync(
                x => x.ApplicationUserId == userId);

            if (guest == null || reservation.GuestId != guest.Id)
                throw new NotAllowedException("You cannot view another guest's reservation.");
        }
        else if (userRole == "Manager")
        {
            var managerHotelId = await GetManagerHotelIdAsync(userId, userRole);
            var belongsToManagerHotel = reservation.ReservationRooms.Any(
                rr => rr.Room.HotelId == managerHotelId);

            if (!belongsToManagerHotel)
                throw new NotAllowedException("You cannot view a reservation for another hotel.");
        }

        return _mapper.Map<ReservationForGettingDto>(reservation);
    }



    public async Task<int> UpdateReservationAsync(
        ReservationForUpdatingDto model,
        string userId,
        string userRole)
    {
        if (model == null)
            throw new BadRequestException(
                "Request model is required.");

        if (model.CheckInDate.Date < DateTime.UtcNow.Date)
            throw new BadRequestException(
                "Check-in date cannot be before today.");

        if (model.CheckOutDate <= model.CheckInDate)
            throw new BadRequestException(
                "Check-out date must be after check-in date.");

        var managerHotelId = await GetManagerHotelIdAsync(
                userId,
                userRole);

        var reservation = await _reservationRepository.GetAsync(
            x => x.Id == model.Id,
            include: query =>
                query.Include(x => x.ReservationRooms)
        );


        if (reservation == null)
            throw new NotFoundException(
                "Reservation not found.");


 
        if (userRole == "Guest")
        {
            var guest = await _guestRepository.GetAsync(
                x => x.ApplicationUserId == userId);

            if (guest == null)
                throw new NotFoundException(
                    "Guest profile not found.");

            if (reservation.GuestId != guest.Id)
            {
                throw new NotAllowedException(
                    "You cannot update another guest's reservation.");
            }
        }


        var roomIds = reservation.ReservationRooms
            .Select(x => x.RoomId)
            .ToList();


        if (managerHotelId.HasValue)
        {
            var roomsBelongToOtherHotel =
                await _roomRepository.ExistsAsync(
                    room =>
                        roomIds.Contains(room.Id) &&
                        room.HotelId != managerHotelId.Value);

            if (roomsBelongToOtherHotel)
            {
                throw new NotAllowedException(
                    "You cannot update a reservation for another hotel.");
            }
        }


        var hasConflict =
            await _reservationRoomRepository.ExistsAsync(
                rr =>
                    roomIds.Contains(rr.RoomId) &&
                    rr.ReservationId != model.Id &&
                    rr.Reservation.Status != ReservationStatus.Cancelled &&
                    rr.Reservation.CheckInDate < model.CheckOutDate &&
                    rr.Reservation.CheckOutDate > model.CheckInDate
            );


        if (hasConflict)
        {
            throw new BadRequestException(
                "One or more rooms are not available for the selected dates.");
        }


        reservation.CheckInDate = model.CheckInDate;
        reservation.CheckOutDate = model.CheckOutDate;

        
        var now = DateTime.UtcNow;

        if (reservation.Status != ReservationStatus.Cancelled)
        {
            if (reservation.CheckInDate > now)
                reservation.Status = ReservationStatus.Reserved;
            else if (reservation.CheckInDate <= now && reservation.CheckOutDate > now)
                reservation.Status = ReservationStatus.Active;
            else
                reservation.Status = ReservationStatus.Completed;
        }

        await _reservationRepository.SaveAsync();

        return reservation.Id;
    }


    private async Task<int?> GetManagerHotelIdAsync(
    string userId,
    string userRole)
    {
        if (userRole != "Manager")
            return null;

        var manager = await _managerRepository.GetAsync(
            x => x.ApplicationUserId == userId);

        if (manager == null)
            throw new NotFoundException("Manager profile not found.");

        return manager.HotelId;
    }
}