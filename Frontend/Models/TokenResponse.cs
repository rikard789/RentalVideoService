using System;
using System.Collections.Generic;

namespace Frontend.Models
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
    }
}
