using HMS.Application.Models.RoomDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Service
{
    public interface IRoomService
    {
        Task<int> CreateRoomAsync(RoomForCreatingDto model);

    }
}
