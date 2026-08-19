using HMS.Application.Models.AuthDtos;
using HMS.Application.Models.GuestDtos;
using HMS.Application.Models.HotelDtos;
using HMS.Application.Models.ManagerDtos;
using HMS.Application.Models.ReservationDtos;


//using HMS.Application.Models.ManagerDtos;
using HMS.Application.Models.RoomDtos;
using HMS.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Mapping
{
    public class MappingConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // HOTEL MAPPING

            config.NewConfig<HotelForCreatingDto, Hotel>();
            config.NewConfig<HotelForUpdatingDto, Hotel>();
            config.NewConfig<Hotel, HotelForGettingDto>();


            // ROOM MAPPING
            config.NewConfig<RoomForCreatingDto, Room>();
            config.NewConfig<RoomForUpdatingDto, Room>();
            config.NewConfig<Room, RoomForGettingDto>();



            // MANAGER MAPPING
            config.NewConfig<ManagerRegistrationRequestDto, Manager>();
            config.NewConfig<Manager, ManagerListForGettingDto>();


            // ROLES MAPPING
            config.NewConfig<ManagerRegistrationRequestDto, ApplicationUser>()
                .Map(dest => dest.UserName, src => src.Email)
                .Map(dest => dest.NormalizedUserName, src => src.Email.ToUpper())
                .Map(dest => dest.NormalizedEmail, src => src.Email.ToUpper())
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.PersonalNumber, src => src.PersonalNumber);

            config.NewConfig<AdminRegistrationRequestDto, ApplicationUser>()
                .Map(dest => dest.UserName, src => src.Email)
                .Map(dest => dest.NormalizedUserName, src => src.Email.ToUpper())
                .Map(dest => dest.NormalizedEmail, src => src.Email.ToUpper())
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.PersonalNumber, src => src.PersonalNumber);

            config.NewConfig<GuestRegistrationRequestDto, ApplicationUser>()
                .Map(dest => dest.UserName, src => src.Email)
                .Map(dest => dest.NormalizedUserName, src => src.Email.ToUpper())
                .Map(dest => dest.NormalizedEmail, src => src.Email.ToUpper())
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.PersonalNumber, src => src.PersonalNumber);

            // GUEST MAPPING
            config.NewConfig<GuestRegistrationRequestDto, Guest>();
            config.NewConfig<Guest, GuestForGettingDto>();
            config.NewConfig<GuestForUpdatingDto, Guest>();
            config.NewConfig<GuestForGettingDto, GuestForUpdatingDto>();


            //RESERVATION MAPPING
            config.NewConfig<ReservationForGettingDto,Reservation>();

        }
    }
}
