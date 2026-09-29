using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.RoomDtos
{
    public class RoomDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public double Price { get; set; }

        public string PrimaryImageUrl { get; set; }

        public List<RoomImageDto> Images { get; set; } = new();
        public int HotelId { get; set; }
    }
}
