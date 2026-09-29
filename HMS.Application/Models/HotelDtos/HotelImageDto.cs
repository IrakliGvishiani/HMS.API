using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.HotelDtos
{
    public class HotelImageDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsPrimary { get; set; }
    }
}
