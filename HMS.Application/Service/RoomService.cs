using HMS.Application.Contracts.Persistance;
using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.RoomDtos;
using HMS.Domain.Entities;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Service
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IHotelService _hotelService;
        private readonly IReservationRoomRepository _reservationRoomRepository;
        private readonly IManagerRepository _managerRepository;
        private readonly IMapper _mapper;
        public RoomService(IRoomRepository roomRepository, IHotelService hotelService, IReservationRoomRepository reservationRoomRepository, IManagerRepository managerRepository, IMapper mapper)
        {
            _roomRepository = roomRepository;
            _hotelService = hotelService;
            _reservationRoomRepository = reservationRoomRepository;
            _managerRepository = managerRepository;
            _mapper = mapper;
        }
        public async Task<int> CreateRoomAsync(RoomForCreatingDto model, string userId)
        {
            if(model == null) throw new BadRequestException("Request Model Required!");

            var manager = await _managerRepository.GetAsync(m => m.ApplicationUserId == userId);

            if (manager == null)
                throw new NotFoundException("Manager not found.");

            if (manager.HotelId != model.HotelId)
                throw new NotAllowedException(
                    "You cannot create a room for another hotel.");

            if (model.Name.Length == 0 || string.IsNullOrWhiteSpace(model.Name)) throw new BadRequestException("Room Name is Required!");

            if(model.Name.Length > 100) throw new BadRequestException("Room Name is too long!");

            if(model.HotelId <= 0) throw new BadRequestException("Invalid Hotel Id!");

            var hotel = await _hotelService.GetHotelAsync(model.HotelId);

            if (hotel == null) throw new NotFoundException("Hotel not found!");



            var mappedRoom = _mapper.Map<Room>(model);

            await _roomRepository.AddAsync(mappedRoom);
            await _roomRepository.SaveAsync();
            return mappedRoom.Id;
        }

        public async Task<int> DeleteRoomAsync(int id, string userId)
        {
            var room = await _roomRepository.GetAsync(x => x.Id == id);


            var manager = await _managerRepository.GetAsync(m => m.ApplicationUserId == userId);

            if (manager == null)
                throw new NotFoundException("Manager not found.");

            if (room.HotelId != manager.HotelId)
                throw new NotAllowedException(
                    "You cannot delete a room from another hotel.");

            bool hasReservations = await _reservationRoomRepository.ExistsAsync(rr =>
    rr.RoomId == id &&
    rr.Reservation.CheckOutDate >= DateTime.UtcNow.Date);


            if (hasReservations)
            {
                throw new BadRequestException("Room cannot be deleted because it has active reservations.");
            }
            
            if (room == null) throw new NotFoundException("Room not found!");
            _roomRepository.Remove(room);
            await _roomRepository.SaveAsync();
            return room.Id;
        }

        public async Task<IEnumerable<RoomForGettingDto>> GetRoomsByHotelIdAsync(int hotelId)
        {
            var rooms = await _roomRepository.GetAllAsync(filter: r => r.HotelId == hotelId, tracking: false);
            var mappedrooms = _mapper.Map<IEnumerable<RoomForGettingDto>>(rooms.Items);
            return mappedrooms;
        }

        public async Task<IEnumerable<RoomForGettingDto>> SearchRoomsAsync(SearchRoomDto model)
        {

            var (rooms, _) = await _roomRepository.GetAllAsync(
     filter: room =>
         room.Price >= model.MinPrice &&
         room.Price <= model.MaxPrice &&
         !room.ReservationRooms.Any(rr =>
             rr.Reservation.CheckInDate < model.CheckOutDate &&
             rr.Reservation.CheckOutDate > model.CheckInDate),
     tracking: false
 );

            return _mapper.Map<IEnumerable<RoomForGettingDto>>(rooms);

        }

        public async Task<RoomForUpdatingDto> UpdateRoomAsync(RoomForUpdatingDto model, string userId)
        {
           if(model == null) throw new BadRequestException("Request Model Required!");

            if(model.Name.Length == 0 || string.IsNullOrWhiteSpace(model.Name)) throw new BadRequestException("Room Name is Required!");

            if(model.Name.Length > 100) throw new BadRequestException("Room Name is too long!");

            if(model.Price <= 0) throw new BadRequestException("Invalid Room Price!");

            var room = await _roomRepository.GetAsync(x => x.Id == model.Id);

            if (room == null) throw new NotFoundException("Room not found!");

            var manager = await _managerRepository.GetAsync(
            x => x.ApplicationUserId == userId);

            if (manager == null)
                throw new NotFoundException("Manager not found.");

            if (room.HotelId != manager.HotelId)
                throw new NotAllowedException(
                    "You cannot update a room from another hotel.");

            var mappedRoom = _mapper.Map(model, room);

            _roomRepository.Update(mappedRoom);
            await _roomRepository.SaveAsync();

            return _mapper.Map<RoomForUpdatingDto>(mappedRoom);
        }

  
    }
}
