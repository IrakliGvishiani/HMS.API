using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.RoomDtos
{
    public class SearchRoomDto
    {

        public int? HotelId { get; set; }
        public double MinPrice { get; set; } = 0;

        public double MaxPrice { get; set; } = 10000000;

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }
    }
}
