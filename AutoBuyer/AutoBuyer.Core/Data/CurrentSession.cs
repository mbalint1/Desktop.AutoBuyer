using AutoBuyer.Core.Models;

namespace AutoBuyer.Core.Data
{
    public static class CurrentSession
    {
        public static SessionDTO Current { get; set; }

        public static void Reset()
        {
            Current = new SessionDTO();
        }
    }
}