using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.RoomDtos
{
    public class RoomImageDto
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public bool IsPrimary { get; set; }
    }
}
