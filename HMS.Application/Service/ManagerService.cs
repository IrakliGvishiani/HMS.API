using HMS.Application.Contracts.Persistance;
using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.AuthDtos;
using HMS.Application.Models.ManagerDtos;
using HMS.Domain.Entities;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Service
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository managerRepository;
        private readonly IHotelService hotelService;
        private readonly IMapper mapper;

        public ManagerService(IManagerRepository managerRepository, IHotelService hotelService, IMapper mapper)
        {
            this.managerRepository = managerRepository;
            this.hotelService = hotelService;
            this.mapper = mapper;
        }
        public async Task<int> CreateManagerAsync(Manager model)
        {
            if(model == null) throw new BadRequestException("Model is required");

            if(string.IsNullOrEmpty(model.FirstName))
                throw new BadRequestException("First name is required");

            if(model.FirstName.Length < 2 || model.FirstName.Length > 100)
                throw new BadRequestException("First name must be between 2 and 100 characters");

            if(string.IsNullOrEmpty(model.LastName))
                throw new BadRequestException("Last name is required");

            if(model.LastName.Length < 2 || model.LastName.Length > 100)
                throw new BadRequestException("Last name must be between 2 and 100 characters");

            if(string.IsNullOrEmpty(model.PersonalNumber))
                throw new BadRequestException("Personal number is required");

            if(model.PersonalNumber.Length !=11)
                throw new BadRequestException("Personal number must be 11 characters long");

            if(string.IsNullOrEmpty(model.Email))
                throw new BadRequestException("Email is required");
            if(!model.Email.Contains("@"))
                throw new BadRequestException("Invalid email format");

            if(string.IsNullOrEmpty(model.PhoneNumber))
                throw new BadRequestException("Phone number is required");
            if(model.PhoneNumber.Length !=9)
                throw new BadRequestException("Phone number must be 9 characters long");

            if(model.HotelId <= 0)
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
            var manager = await managerRepository.GetAsync(s => s.Id == id);
            if (manager == null)
                throw new NotFoundException("Manager not found");

            var hasAnotherManager = await managerRepository.ExistsAsync(
       x => x.HotelId == manager.HotelId &&
            x.Id != manager.Id);

            if (!hasAnotherManager)
                throw new BadRequestException(
                    "Manager cannot be deleted because this hotel must have at least one manager.");


            managerRepository.Remove(manager);
            await managerRepository.SaveAsync();
            return id;
        }

        public async Task<IEnumerable<ManagerListForGettingDto>> GetManagersAsync()
        {
            var managers = await managerRepository.GetAllAsync();
            var managerDtos = mapper.Map<IEnumerable<ManagerListForGettingDto>>(managers.Items);
            return managerDtos;
        }
    }
}
