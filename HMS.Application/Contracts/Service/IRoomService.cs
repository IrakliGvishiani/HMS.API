using HMS.Application.Models.RoomDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Service
{
    public interface IRoomService
    {
        Task<int> CreateRoomAsync(RoomForCreatingDto model, string userId);

        Task<RoomForUpdatingDto> UpdateRoomAsync(RoomForUpdatingDto model, string userId);

        Task<IEnumerable<RoomForGettingDto>> SearchRoomsAsync(SearchRoomDto model);

        Task<int> DeleteRoomAsync(int id, string userId);

        Task<IEnumerable<RoomForGettingDto>> GetRoomsByHotelIdAsync(int hotelId);
    }
}
