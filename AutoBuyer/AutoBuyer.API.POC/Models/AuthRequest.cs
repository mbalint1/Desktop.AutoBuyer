namespace AutoBuyer.API.POC.Models
{
    public class AuthRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string AppName { get; set; } // might use this eventually for mobile
    }
}