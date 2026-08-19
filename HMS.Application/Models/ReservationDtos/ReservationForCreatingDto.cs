using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.ReservationDtos
{
    public class ReservationForCreatingDto
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public List<int> RoomIds { get; set; }
    }
}
