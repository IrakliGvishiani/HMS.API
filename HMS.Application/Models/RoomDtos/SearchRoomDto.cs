using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.RoomDtos
{
    public class SearchRoomDto
    {
        public double MinPrice { get; set; }

        public double MaxPrice { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }
    }
}
