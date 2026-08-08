using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.AuthDtos
{
    public class AdminRegistrationRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string PersonalNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
}
