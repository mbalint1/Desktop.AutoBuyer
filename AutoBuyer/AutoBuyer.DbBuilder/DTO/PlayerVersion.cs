using System;

namespace AutoBuyer.Data.DTO
{
    public class PlayerVersion
    {
        public string VersionId { get; set; }

        public string PlayerId { get; set; }

        public string ThirdPartyId { get; set; }

        public int Rating { get; set; }

        public string Position { get; set; }

        public PlayerType Type { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}