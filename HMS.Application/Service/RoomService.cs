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
        private readonly IMapper _mapper;
        public RoomService(IRoomRepository roomRepository, IHotelService hotelService, IMapper mapper)
        {
            _roomRepository = roomRepository;
            _hotelService = hotelService;
            _mapper = mapper;
        }
        public async Task<int> CreateRoomAsync(RoomForCreatingDto model)
        {
            if(model == null) throw new BadRequestException("Request Model Required!");

            if(model.Name.Length == 0 || string.IsNullOrWhiteSpace(model.Name)) throw new BadRequestException("Room Name is Required!");

            if(model.Name.Length > 100) throw new BadRequestException("Room Name is too long!");

            if(model.HotelId <= 0) throw new BadRequestException("Invalid Hotel Id!");

            var hotel = await _hotelService.GetHotelAsync(model.HotelId);

            if (hotel == null) throw new NotFoundException("Hotel not found!");



            var mappedRoom = _mapper.Map<Room>(model);

            await _roomRepository.AddAsync(mappedRoom);
            await _roomRepository.SaveAsync();
            return mappedRoom.Id;
        }
    }
}
