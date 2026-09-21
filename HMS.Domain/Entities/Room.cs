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

        public ICollection<ReservationRoom> ReservationRooms { get; set; } = new List<ReservationRoom>();

        public ICollection<RoomImage> RoomImages { get; set; } = new List<RoomImage>();
    }
}
