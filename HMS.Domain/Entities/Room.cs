using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Domain.Entities
{
    public class Room
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public int HotelId { get; set; }

        public Hotel Hotel { get; set; }

        public IEnumerable<ReservationRoom> ReservationRooms { get; set; }
    }
}
