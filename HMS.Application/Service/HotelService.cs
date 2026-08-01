using HMS.Application.Contracts.Persistance;
using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.HotelDtos;
using MapsterMapper;
using System;
using System.Collections.Generic;
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

           var mappedHotel = _mapper.Map<Domain.Entities.Hotel>(model);

            await _hotelRepository.AddAsync(mappedHotel);
            await _hotelRepository.SaveAsync();
            return mappedHotel.Id;

        }
    }
}
