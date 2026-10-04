using HMS.Application.Contracts.Persistance;
using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.RoomDtos;
using HMS.Domain.Entities;
using HMS.Domain.Enum;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HMS.Application.Service
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IHotelService _hotelService;
        private readonly IReservationRoomRepository _reservationRoomRepository;
        private readonly IManagerRepository _managerRepository;
        private readonly ICloudinaryImageService _cloudinaryImageService;
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        private const int maxImages = 25;
        public RoomService(IRoomRepository roomRepository,
            IHotelService hotelService,
            IReservationRoomRepository reservationRoomRepository,
            IManagerRepository managerRepository,
            ICloudinaryImageService cloudinaryImageService,
            IAdminRepository adminRepository,
            IMapper mapper)
        {
            _roomRepository = roomRepository;
            _hotelService = hotelService;
            _reservationRoomRepository = reservationRoomRepository;
            _managerRepository = managerRepository;
            _cloudinaryImageService = cloudinaryImageService;
            _adminRepository = adminRepository;
            _mapper = mapper;
        }
        public async Task<int> CreateRoomAsync(RoomForCreatingDto model, string userId, CancellationToken ct = default)
        {
            if(model == null) throw new BadRequestException("Request Model Required!");

            var manager = await _managerRepository.GetAsync(m => m.ApplicationUserId == userId);
            var admin = await _adminRepository.GetAsync(a => a.ApplicationUserId == userId);

            if (manager == null && admin == null)
                throw new NotFoundException("User not found.");

            if (manager != null && manager.HotelId != model.HotelId)
                throw new NotAllowedException(
                    "You cannot create a room for another hotel.");

            if (model.Name.Length == 0 || string.IsNullOrWhiteSpace(model.Name)) throw new BadRequestException("Room Name is Required!");

            if(model.Name.Length > 100) throw new BadRequestException("Room Name is too long!");

            if(model.HotelId <= 0) throw new BadRequestException("Invalid Hotel Id!");

            var hotel = await _hotelService.GetHotelAsync(model.HotelId);

            if (hotel == null) throw new NotFoundException("Hotel not found!");

            if(model.Images != null && model.Images.Count > maxImages)
                throw new BadRequestException($"You can upload up to {maxImages} images.");

            var mappedRoom = _mapper.Map<Room>(model);

            if (model.Images != null && model.Images.Any(f => f != null && f.Length > 0))
            {
                var roomImages = new List<RoomImage>();
                var validFiles = model.Images.Where(f => f != null && f.Length > 0).ToList();

                for (int i = 0; i < validFiles.Count; i++)
                {
                    var uploadResult = await _cloudinaryImageService.UploadAsync(
                        validFiles[i], width: 1200, height: 800, folder: "rooms", ct
                        );

                    roomImages.Add(new RoomImage
                    {
                        ImageUrl = uploadResult.Url,
                        ImagePublicId = uploadResult.publicId,
                        IsPrimary = i == 0
                    });
                }

                mappedRoom.RoomImages = roomImages;
            }


            await _roomRepository.AddAsync(mappedRoom);
            await _roomRepository.SaveAsync();
            return mappedRoom.Id;
        }

        public async Task<int> DeleteRoomAsync(int id, string userId)
        {
            
            var room = await _roomRepository.GetAsync(
                x => x.Id == id,
                include: query => query.Include(r => r.RoomImages));

            if (room == null)
                throw new NotFoundException("Room not found!");

            
            var manager = await _managerRepository.GetAsync(m => m.ApplicationUserId == userId);
            var admin = await _adminRepository.GetAsync(a => a.ApplicationUserId == userId);

            if (manager == null && admin == null)
                throw new NotFoundException("User not found.");

           
            if (manager != null && room.HotelId != manager.HotelId)
                throw new NotAllowedException("You cannot delete a room from another hotel.");

            
            bool hasReservations = await _reservationRoomRepository.ExistsAsync(rr =>
                rr.RoomId == id &&
                rr.Reservation.CheckOutDate >= DateTime.UtcNow.Date);

            if (hasReservations)
            {
                throw new BadRequestException("Room cannot be deleted because it has active reservations.");
            }

            
            if (room.RoomImages != null && room.RoomImages.Any())
            {
                foreach (var image in room.RoomImages)
                {
                    if (!string.IsNullOrEmpty(image.ImagePublicId))
                    {
                        await _cloudinaryImageService.DeleteAsync(image.ImagePublicId);
                    }
                }
            }

            
            _roomRepository.Remove(room);
            await _roomRepository.SaveAsync();

            return room.Id;
        }

        public async Task<IEnumerable<RoomForGettingDto>> GetRoomsByHotelIdAsync(int hotelId)
        {
            var rooms = await _roomRepository.GetAllAsync(filter: r => r.HotelId == hotelId,
                includes: q => q.Include(r => r.RoomImages),

                tracking: false);
            var mappedrooms = _mapper.Map<IEnumerable<RoomForGettingDto>>(rooms.Items);
            return mappedrooms;
        }

        public async Task<IEnumerable<RoomForGettingDto>> SearchRoomsAsync(SearchRoomDto model)
        {
            if (model.CheckOutDate <= model.CheckInDate)
                throw new ArgumentException("Check-out date must be after check-in date");

            var (rooms, _) = await _roomRepository.GetAllAsync(
                filter: room =>
                    (!model.HotelId.HasValue || room.HotelId == model.HotelId) &&
                    room.Price >= model.MinPrice &&
                    room.Price <= model.MaxPrice &&
                    !room.ReservationRooms.Any(rr =>
                        rr.Reservation.Status != ReservationStatus.Cancelled &&
                        rr.Reservation.CheckInDate < model.CheckOutDate &&
                        rr.Reservation.CheckOutDate > model.CheckInDate),
                includes: q => q.Include(r => r.RoomImages),
                tracking: false
            );

            return _mapper.Map<IEnumerable<RoomForGettingDto>>(rooms);
        }

        public async Task<RoomForUpdatingDto> UpdateRoomAsync(
            RoomForUpdatingDto model,
            string userId,
            CancellationToken ct = default)
        {
            if (model == null)
                throw new BadRequestException("Request Model Required!");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new BadRequestException("Room Name is Required!");

            if (model.Name.Length > 100)
                throw new BadRequestException("Room Name is too long!");

            if (model.Price <= 0)
                throw new BadRequestException("Invalid Room Price!");

            var room = await _roomRepository.GetAsync(
                x => x.Id == model.Id,
                include: query => query.Include(x => x.RoomImages));

            if (room == null)
                throw new NotFoundException("Room not found!");

            var manager = await _managerRepository.GetAsync(
                x => x.ApplicationUserId == userId);
            var  admin = await _adminRepository.GetAsync(a => a.ApplicationUserId == userId);
            if (manager == null && admin == null)
                throw new NotFoundException("User not found.");

            if (manager != null && room.HotelId != manager.HotelId)
                throw new NotAllowedException(
                    "You cannot update a room from another hotel.");

            room.Name = model.Name;
            room.Price = model.Price;

            
            if (model.ImageIdsToDelete != null &&
                model.ImageIdsToDelete.Any())
            {
                var imagesToRemove = room.RoomImages
                    .Where(i => model.ImageIdsToDelete.Contains(i.Id))
                    .ToList();

                foreach (var image in imagesToRemove)
                {
                    if (!string.IsNullOrEmpty(image.ImagePublicId))
                    {
                        await _cloudinaryImageService.DeleteAsync(
                            image.ImagePublicId,
                            ct: ct);
                    }

                    room.RoomImages.Remove(image);
                }
            }

            
            if (model.ImagesToAdd != null &&
                model.ImagesToAdd.Any(f => f != null && f.Length > 0))
            {
                var validFiles = model.ImagesToAdd
                    .Where(f => f != null && f.Length > 0)
                    .ToList();

                foreach (var file in validFiles)
                {
                    var uploadResult =
                        await _cloudinaryImageService.UploadAsync(
                            file,
                            width: 1200,
                            height: 800,
                            folder: "rooms",
                            ct);

                    room.RoomImages.Add(new RoomImage
                    {
                        ImageUrl = uploadResult.Url,
                        ImagePublicId = uploadResult.publicId,
                        IsPrimary = false
                    });
                }
            }

            
            if (model.PrimaryImageId.HasValue)
            {
                var primaryImage = room.RoomImages
                    .FirstOrDefault(i =>
                        i.Id == model.PrimaryImageId.Value);

                if (primaryImage == null)
                    throw new BadRequestException(
                        "Selected primary image was not found.");

                foreach (var image in room.RoomImages)
                {
                    image.IsPrimary = image.Id == primaryImage.Id;
                }
            }

            
            if (room.RoomImages.Any() &&
                !room.RoomImages.Any(i => i.IsPrimary))
            {
                room.RoomImages.First().IsPrimary = true;
            }

            await _roomRepository.SaveAsync();

            return _mapper.Map<RoomForUpdatingDto>(room);
        }

        public async Task<RoomDetailsDto> GetRoomDetailsByIdAsync(int roomId)
        {
            if(roomId <= 0) throw new BadRequestException("Invalid Room ID!");

            var room = await _roomRepository.GetAsync(x => x.Id == roomId,
                include: r => r.Include(room => room.RoomImages));

            if (room == null) throw new NotFoundException("Room not found!");

            return _mapper.Map<RoomDetailsDto>(room);
        }
    }
}
