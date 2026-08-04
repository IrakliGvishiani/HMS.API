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
        public HotelService(IHotelRepository hotelRepository, IMapper mapper)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
        }

        public async Task<int> CreateNewHotelAsync(HotelForCreatingDto model)
        {
            if (model == null) throw new BadRequestException("Request Model Required!");

            if(model.Name.Length == 0 || string.IsNullOrWhiteSpace(model.Name)) throw new BadRequestException("Hotel Name is Required!");

            if(model.Name.Length > 100) throw new BadRequestException("Hotel Name is too long!");

            if(model.Country == null || string.IsNullOrWhiteSpace(model.Country)) throw new BadRequestException("Hotel Country is Required!");

            if(model.Country.Length > 100) throw new BadRequestException("Hotel Country Name is too long!");

            if(model.Address == null || string.IsNullOrWhiteSpace(model.Address)) throw new BadRequestException("Hotel Address is Required!");

            if(model.Address.Length > 200) throw new BadRequestException("Hotel Address is too long!");

            if(model.Rating < 1 || model.Rating > 5) throw new BadRequestException("Hotel Rating is Between 1-5!");

           var mappedHotel = _mapper.Map<Hotel>(model);

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

        public async Task<HotelForGettingDto> GetHotelAsync(int id)
        {
           if(id <= 0) throw new BadRequestException("Invalid Hotel Id!");

           var hotel = await _hotelRepository.GetAsync(f => f.Id == id);
           if(hotel == null) throw new NotFoundException("Hotel not found!");

           return _mapper.Map<HotelForGettingDto>(hotel);
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
                pageSize: parameters.PageSize
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

        public async Task<int> UpdateHotelAsync(HotelForUpdatingDto model)
        {
            if (model.Name.Length == 0 || string.IsNullOrWhiteSpace(model.Name)) throw new BadRequestException("Hotel Name is Required!");

            if (model.Name.Length > 100) throw new BadRequestException("Hotel Name is too long!");

            if (model.Address == null || string.IsNullOrWhiteSpace(model.Address)) throw new BadRequestException("Hotel Address is Required!");

            if (model.Address.Length > 200) throw new BadRequestException("Hotel Address is too long!");

            if (model.Rating < 1 || model.Rating > 5) throw new BadRequestException("Hotel Rating is Between 1-5!");

            var hotel = await _hotelRepository.GetAsync(f => f.Id == model.Id);

            if (hotel == null)
                throw new NotFoundException("Hotel not found!");

            hotel.Name = model.Name;
            hotel.Address = model.Address;
            hotel.Rating = model.Rating;

            await _hotelRepository.SaveAsync();

            return hotel.Id;

        }
    }
}
