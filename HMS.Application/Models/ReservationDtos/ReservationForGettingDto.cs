using HMS.Domain.Enum;
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

        public string GuestName { get; set; }

        public string GuestPhoneNumber { get; set; }

        public List<int> RoomIds { get; set; } = new();

        public int HotelId { get; set; }

        public string HotelName { get; set; }

        public ReservationStatus Status { get; set; }
    }
}
