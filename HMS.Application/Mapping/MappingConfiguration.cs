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
            config.NewConfig<Hotel, HotelForGettingDto>()
            .Map(dest => dest.PrimaryImageUrl, src => src.HotelImages.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault());


            config.NewConfig<Hotel, HotelDetailsDto>()
    .Map(dest => dest.PrimaryImageUrl,
        src => src.HotelImages
            .Where(i => i.IsPrimary)
            .Select(i => i.ImageUrl)
            .FirstOrDefault())
    .Map(dest => dest.Images,
        src => src.HotelImages.Select(image => new HotelImageDto
        {
            Id = image.Id,
            Url = image.ImageUrl,
            IsPrimary = image.IsPrimary
        }).ToList());


            config.NewConfig<HotelImage, HotelImageDto>()
            .Map(dest => dest.Url, src => src.ImageUrl);

            // ROOM MAPPING
            config.NewConfig<RoomForCreatingDto, Room>();
            config.NewConfig<RoomForUpdatingDto, Room>();
            config.NewConfig<Room, RoomForGettingDto>()
                .Map(dest => dest.PrimaryImageUrl, src => src.RoomImages.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault());

            config.NewConfig<RoomImage, RoomImageDto>()
    .Map(dest => dest.Url, src => src.ImageUrl);

            config.NewConfig<Room, RoomDetailsDto>()
    .Map(dest => dest.PrimaryImageUrl,
        src => src.RoomImages != null
            ? src.RoomImages.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault()
            : null)
    .Map(dest => dest.Images,
        src => src.RoomImages != null
            ? src.RoomImages.Select(image => new RoomImageDto
            {
                Id = image.Id,
                Url = image.ImageUrl,
                IsPrimary = image.IsPrimary
            }).ToList()
            : new List<RoomImageDto>());

            // MANAGER MAPPING
            config.NewConfig<ManagerRegistrationRequestDto, Manager>();
            //config.NewConfig<Manager, ManagerListForGettingDto>();

            config.NewConfig<Manager, ManagerListForGettingDto>()
    .Map(dest => dest.Id, src => src.Id)
    .Map(dest => dest.FirstName, src => src.FirstName)
    .Map(dest => dest.LastName, src => src.LastName)
    .Map(dest => dest.PersonalNumber, src => src.ApplicationUser.PersonalNumber)
    .Map(dest => dest.Email, src => src.ApplicationUser.Email)
    .Map(dest => dest.PhoneNumber, src => src.ApplicationUser.PhoneNumber)
    .Map(dest => dest.HotelId, src => src.HotelId)             
    .Map(dest => dest.HotelName, src => src.Hotel.Name);

            config.NewConfig<Manager, ManagerProfileDto>()
    .Map(dest => dest.Email, src => src.ApplicationUser.Email)
    .Map(dest => dest.PersonalNumber, src => src.ApplicationUser.PersonalNumber)
    .Map(dest => dest.PhoneNumber, src => src.ApplicationUser.PhoneNumber)
    .Map(dest => dest.HotelName, src => src.Hotel.Name);

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
            config.NewConfig<Guest, GuestForGettingDto>()
    .Map(dest => dest.Email, src => src.ApplicationUser.Email)
    .Map(dest => dest.PersonalNumber, src => src.ApplicationUser.PersonalNumber)
    .Map(dest => dest.PhoneNumber, src => src.ApplicationUser.PhoneNumber);
            config.NewConfig<GuestForUpdatingDto, Guest>();
            config.NewConfig<GuestForGettingDto, GuestForUpdatingDto>();


            //RESERVATION MAPPING
            //config.NewConfig<ReservationForGettingDto,Reservation>();

            config.NewConfig<ReservationRoom, ReservationRoomInfoDto>()
                .Map(dest => dest.RoomName, src => src.Room.Name)
                .Map(dest => dest.PricePerNight, src => src.Room.Price);

            config.NewConfig<Reservation, ReservationForGettingDto>()
                .Map(
                    dest => dest.GuestName,
                    src => $"{src.Guest.FirstName} {src.Guest.LastName}"
                )
                .Map(
                    dest => dest.GuestPhoneNumber,
                    src => src.Guest.ApplicationUser.PhoneNumber
                )
                .Map(
                    dest => dest.RoomIds,
                    src => src.ReservationRooms.Select(rr => rr.RoomId).ToList()
                )
                .Map(
                    dest => dest.HotelId,
                    src => src.ReservationRooms.Select(rr => rr.Room.HotelId).FirstOrDefault()
                )
                .Map(
                    dest => dest.HotelName,
                    src => src.ReservationRooms.Select(rr => rr.Room.Hotel.Name).FirstOrDefault()
                )
                .Map(
                    dest => dest.Rooms,
                    src => src.ReservationRooms.Select(rr => new ReservationRoomInfoDto
                    {
                        RoomId = rr.RoomId,
                        RoomName = rr.Room.Name,
                        PricePerNight = rr.Room.Price
                    }).ToList()
                )
                .Map(
                    dest => dest.Nights,
                    src => (src.CheckOutDate.Date - src.CheckInDate.Date).Days
                )
                .Map(
                    dest => dest.TotalPrice,
                    src => src.ReservationRooms.Sum(rr => rr.Room.Price) * (src.CheckOutDate.Date - src.CheckInDate.Date).Days
                );

            //ADMIN
            config.NewConfig<Admin, AdminForGettingDto>()
                .Map(dest => dest.Email, src => src.ApplicationUser.Email)
                .Map(dest => dest.PersonalNumber, src => src.ApplicationUser.PersonalNumber)
                .Map(dest => dest.PhoneNumber, src => src.ApplicationUser.PhoneNumber);

        }
    }
}
