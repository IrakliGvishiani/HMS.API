using HMS.Application.Models.HotelDtos;
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
        }
    }
}
