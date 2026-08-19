using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string PersonalNumber { get; set; }
        public Manager Manager { get; set; }

        public Admin Admin { get; set; }

        public Guest Guest { get; set; }
    }
}
