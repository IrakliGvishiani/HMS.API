using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.AuthDtos
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
