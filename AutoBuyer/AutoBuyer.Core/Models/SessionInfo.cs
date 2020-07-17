using System;

namespace AutoBuyer.Core.Models
{
    public class SessionInfo
    {
        public string PlayerVersionId { get; set; }

        public bool EndSession { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}