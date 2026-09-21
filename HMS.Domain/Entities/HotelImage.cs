using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HMS.Domain.Entities
{
    public class HotelImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string ImagePublicId { get; set; }

        public bool IsPrimary { get; set; }

        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}
