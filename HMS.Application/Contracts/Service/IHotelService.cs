using HMS.Application.Models.Common;
using HMS.Application.Models.HotelDtos;
using HMS.Application.Models.RoomDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Service
{
    public interface IHotelService
    {

        Task<int> CreateNewHotelAsync(HotelForCreatingDto model);

        Task<int> UpdateHotelAsync(HotelForUpdatingDto model);

        Task<HotelForGettingDto> GetHotelAsync(int id);

        Task<PagedResponseDto<HotelForGettingDto>> GetHotelListAsync(PagedRequestDto parameters);

        Task<int> DeleteHotelAsync(int id);

    }
}
