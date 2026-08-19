using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.ReservationDtos
{
    public class ReservationSearchDto
    {
       
            public int? HotelId { get; set; }

            public int? GuestId { get; set; }

            public int? RoomId { get; set; }

            public DateTime? Date { get; set; }

            public bool? Active { get; set; }
        
    }
}
