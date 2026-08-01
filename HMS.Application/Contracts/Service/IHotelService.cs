using HMS.Application.Models.HotelDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Service
{
    public interface IHotelService
    {

        Task<int> CreateNewHotelAsync(HotelForCreatingDto model); 
    }
}
