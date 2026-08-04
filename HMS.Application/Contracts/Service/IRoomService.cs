using HMS.Application.Models.RoomDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Service
{
    public interface IRoomService
    {
        Task<int> CreateRoomAsync(RoomForCreatingDto model);

        Task<RoomForUpdatingDto> UpdateRoomAsync(RoomForUpdatingDto model);

        Task<IEnumerable<RoomForGettingDto>> SearchRoomsAsync(SearchRoomDto model);

        Task<int> DeleteRoomAsync(int id);

        Task<IEnumerable<RoomForGettingDto>> GetRoomsByHotelIdAsync(int hotelId);
    }
}
