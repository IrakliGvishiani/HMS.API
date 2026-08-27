using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.Analytics
{
  public class HotelAnalyticsDto
    {
        public int HotelId { get; set; }

        public int TotalRooms { get; set; }

        public int AvailableRooms { get; set; }

        public int ReservedRooms { get; set; }

        public int OccupiedRooms { get; set; }

        public int TotalReservations { get; set; }

        public int ActiveReservations { get; set; }

        public int CancelledReservations { get; set; }

        public int CompletedReservations { get; set; }

        public int TotalGuests { get; set; }

        public double TotalRevenue { get; set; }
    }
}
