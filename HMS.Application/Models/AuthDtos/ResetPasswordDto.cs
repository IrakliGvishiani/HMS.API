using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.AuthDtos
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }

        public string Token { get; set; }

        public string NewPassword { get; set; }
    }
}
