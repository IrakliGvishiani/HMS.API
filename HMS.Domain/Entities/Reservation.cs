using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public int GuestId { get; set; }

        public Guest Guest { get; set; }

        public ICollection<ReservationRoom> ReservationRooms { get; set; }
     = new List<ReservationRoom>();
    }
}
