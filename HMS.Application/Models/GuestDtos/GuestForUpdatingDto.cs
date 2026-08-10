using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.GuestDtos
{
    public class GuestForUpdatingDto
    {

        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PersonalNumber { get; set; }

        public string PhoneNumber { get; set; }

    }
}
