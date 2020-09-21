namespace AutoBuyer.Core.Models
{
    public class SessionDTO
    {
        public string SessionId { get; set; }

        public string PlayerVersionId { get; set; }

        public bool Captcha { get; set; }

        public int SearchNum { get; set; }

        public int PurchasedNum { get; set; }

        public string AccessToken { get; set; }
    }
}