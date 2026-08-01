using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Domain.Entities
{
    public class Hotel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public byte Rating { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public IEnumerable<Manager> Managers { get; set; }

        public IEnumerable<Room> Rooms { get; set; }
    }
}
