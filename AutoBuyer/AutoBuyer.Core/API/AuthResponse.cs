using System;

namespace AutoBuyer.Core.API
{
    public class AuthResponse
    {
        public string Username { get; set; }

        public string AccessToken { get; set; }

        public DateTime TokenIssueDate { get; set; }

        public DateTime TokenExpirationDate { get; set; }

        public bool Authenticated { get; set; }
    }
}