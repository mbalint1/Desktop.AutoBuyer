using System;

namespace AutoBuyer.Core.Data
{
    public static class CurrentSession
    {
        public static string Token { get; set; }

        public static string PlayerVersionId { get; set; }

        public static string SessionId { get; set; }
    }
}