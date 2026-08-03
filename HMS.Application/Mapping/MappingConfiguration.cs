using HMS.Application.Models.HotelDtos;
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
        }
    }
}
