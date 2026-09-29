using HMS.Application.Contracts.Persistance;
using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.Common;
using HMS.Application.Models.HotelDtos;
using HMS.Domain.Entities;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HMS.Application.Service
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;
        private readonly ICloudinaryImageService _cloudinaryImageService;
        private readonly IManagerRepository _managerRepository;
        private const int maxImages = 15;
        public HotelService(IHotelRepository hotelRepository, IMapper mapper,
            ICloudinaryImageService cloudinaryImageService,
            IManagerRepository managerRepository)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
            _cloudinaryImageService = cloudinaryImageService;
            _managerRepository = managerRepository;
        }

        public async Task<int> CreateNewHotelAsync(HotelForCreatingDto model, CancellationToken ct = default)
        {
            if (model == null) throw new BadRequestException("Request Model Required!");

            if(model.Name.Length == 0 || string.IsNullOrWhiteSpace(model.Name)) throw new BadRequestException("Hotel Name is Required!");

            if(model.Name.Length > 100) throw new BadRequestException("Hotel Name is too long!");

            if(model.Country == null || string.IsNullOrWhiteSpace(model.Country)) throw new BadRequestException("Hotel Country is Required!");

            if(model.Country.Length > 100) throw new BadRequestException("Hotel Country Name is too long!");

            if(model.Address == null || string.IsNullOrWhiteSpace(model.Address)) throw new BadRequestException("Hotel Address is Required!");

            if(model.Address.Length > 200) throw new BadRequestException("Hotel Address is too long!");

            if(model.Rating < 1 || model.Rating > 5) throw new BadRequestException("Hotel Rating is Between 1-5!");

            if(model.Images != null && model.Images.Count > maxImages)
                throw new BadRequestException($"You can upload up to {maxImages} images.");

            var mappedHotel = _mapper.Map<Hotel>(model);

            if (model.Images != null && model.Images.Any(f => f != null && f.Length > 0))
            {
                var hotelImages = new List<HotelImage>();
                var validFiles = model.Images.Where(f => f != null && f.Length > 0).ToList();

                for(int i = 0;i < validFiles.Count; i++)
                {
                    var uploadResult = await _cloudinaryImageService.UploadAsync(
                        validFiles[i],width: 1200, height: 800,folder: "hotels",ct
                        );

                    hotelImages.Add(new HotelImage
                    {
                        ImageUrl = uploadResult.Url,
                        ImagePublicId = uploadResult.publicId,
                        IsPrimary = i == 0 
                    });
                }

                mappedHotel.HotelImages = hotelImages;
            }

            

            await _hotelRepository.AddAsync(mappedHotel);
            await _hotelRepository.SaveAsync();
            return mappedHotel.Id;

        }

        public async Task<int> DeleteHotelAsync(int id)
        {
            var hotel = await _hotelRepository.GetAsync(f => f.Id == id,
                include: query => query.Include(h => h.Rooms));
            if (hotel == null) throw new NotFoundException("Hotel not found!");

            if (hotel.Rooms.Any())
            {
                throw new BadRequestException("Hotel cannot be deleted because it has rooms.");
            }

            _hotelRepository.Remove(hotel);
            await _hotelRepository.SaveAsync();
            return hotel.Id;
        }

        public async Task<HotelDetailsDto> GetHotelAsync(int id)
        {
           if(id <= 0) throw new BadRequestException("Invalid Hotel Id!");

           var hotel = await _hotelRepository.GetAsync(f => f.Id == id,
               include: query => query.Include(h => h.HotelImages));
           if(hotel == null) throw new NotFoundException("Hotel not found!");

           return _mapper.Map<HotelDetailsDto>(hotel);
        }

        public async Task<PagedResponseDto<HotelForGettingDto>> GetHotelListAsync(PagedRequestDto parameters)
        {
                Expression<Func<Hotel, object>> orderBy = parameters.SortBy?.ToLower() switch
                {
                    "name" => h => h.Name,
                    "rating" => h => h.Rating,
                    "country" => h => h.Country,
                    "city" => h => h.City,
                    _ => h => h.Id
                };

            var hotels = await _hotelRepository.GetAllAsync(
                filter: h => string.IsNullOrEmpty(parameters.FilterBy) || h.Name.Contains(parameters.FilterBy),
                orderBy: orderBy,
                ascending: parameters.Ascending,
                pageNumber: parameters.PageNumber,
                pageSize: parameters.PageSize,
                includes: q => q.Include(h => h.HotelImages.Where(i => i.IsPrimary))
            );

            if(hotels.Items.Count() == 0)
            {
                return new PagedResponseDto<HotelForGettingDto>
                {
                    Items = new List<HotelForGettingDto>(),
                    TotalCount = 0,
                    PageNumber = parameters.PageNumber,
                    PageSize = parameters.PageSize,
                    
                };
            }
            
            var result = _mapper.Map<IEnumerable<HotelForGettingDto>>(hotels.Items);

            return new PagedResponseDto<HotelForGettingDto>
            {
                Items = result,
                TotalCount = hotels.TotalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };

        }

        public async Task<int> UpdateHotelAsync(HotelForUpdatingDto model,string userId,string userRole, CancellationToken ct = default)
        {
            if (model.Name.Length == 0 || string.IsNullOrWhiteSpace(model.Name)) throw new BadRequestException("Hotel Name is Required!");
            if (model.Name.Length > 100) throw new BadRequestException("Hotel Name is too long!");
            if (model.Address == null || string.IsNullOrWhiteSpace(model.Address)) throw new BadRequestException("Hotel Address is Required!");
            if (model.Address.Length > 200) throw new BadRequestException("Hotel Address is too long!");
            if (model.Rating < 1 || model.Rating > 5) throw new BadRequestException("Hotel Rating is Between 1-5!");

            var hotel = await _hotelRepository.GetAsync(
                f => f.Id == model.Id,
                include: query => query.Include(h => h.HotelImages));

            if (hotel == null)
                throw new NotFoundException("Hotel not found!");

            if (userRole == "Manager")
            {
                var manager = await _managerRepository.GetAsync(x => x.ApplicationUserId == userId);

                if (manager == null || manager.HotelId != hotel.Id)
                    throw new NotAllowedException("You cannot edit a hotel that is not assigned to you.");
            }

            hotel.Name = model.Name;
            hotel.Address = model.Address;
            hotel.Rating = model.Rating;

            if (model.ImageIdsToDelete != null && model.ImageIdsToDelete.Any())
            {
                var imagesToRemove = hotel.HotelImages
                    .Where(i => model.ImageIdsToDelete.Contains(i.Id))
                    .ToList();

                foreach (var image in imagesToRemove)
                {
                    if (!string.IsNullOrEmpty(image.ImagePublicId))
                        await _cloudinaryImageService.DeleteAsync(image.ImagePublicId, ct: ct);

                    hotel.HotelImages.Remove(image);
                }
            }

            if (model.ImagesToAdd != null && model.ImagesToAdd.Any(f => f != null && f.Length > 0))
            {
                var validFiles = model.ImagesToAdd.Where(f => f != null && f.Length > 0).ToList();

                foreach (var file in validFiles)
                {
                    var uploadResult = await _cloudinaryImageService.UploadAsync(file, width: 1200, height: 800, folder: "hotels", ct);

                    hotel.HotelImages.Add(new HotelImage
                    {
                        ImageUrl = uploadResult.Url,
                        ImagePublicId = uploadResult.publicId,
                        IsPrimary = false
                    });
                }
            }

            if (hotel.HotelImages.Any() && !hotel.HotelImages.Any(i => i.IsPrimary))
            {
                hotel.HotelImages.First().IsPrimary = true;
            }

            if (model.PrimaryImageId.HasValue)
            {
                var primaryImage = hotel.HotelImages
                    .FirstOrDefault(i => i.Id == model.PrimaryImageId.Value);

                if (primaryImage == null)
                    throw new BadRequestException("Selected primary image was not found.");

                foreach (var image in hotel.HotelImages)
                {
                    image.IsPrimary = image.Id == primaryImage.Id;
                }
            }

            await _hotelRepository.SaveAsync();

            return hotel.Id;
        }
    }
}
