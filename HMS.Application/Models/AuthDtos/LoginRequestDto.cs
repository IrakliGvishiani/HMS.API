using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.AuthDtos
{
   public class LoginRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
