using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.ReservationDtos
{
    public class ReservationForGettingDto
    {
  
            public int Id { get; set; }

            public DateTime CheckInDate { get; set; }

            public DateTime CheckOutDate { get; set; }

            public int GuestId { get; set; }

            
        
    }
}
