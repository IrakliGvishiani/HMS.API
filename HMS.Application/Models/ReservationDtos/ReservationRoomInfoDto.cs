using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.ReservationDtos
{
    public class ReservationRoomInfoDto
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; } = null!;
        public double PricePerNight { get; set; }
    }
}
