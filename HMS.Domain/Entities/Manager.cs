using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Domain.Entities
{
    public class Manager
    {

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PersonalNumber { get; set; }

        public string Email {  get; set; }

        public string PhoneNumber { get; set; }


        public Hotel Hotel { get; set; }
        public int HotelId { get; set; }
    }
}
