using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Domain.Entities
{
    public class RoomImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string ImagePublicId { get; set; }

        public bool IsPrimary { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
