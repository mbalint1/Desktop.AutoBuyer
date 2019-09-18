using System;
using System.Collections.Generic;

namespace AutoBuyer.DbBuilder.DTO
{
    public class Player
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public List<PlayerVersion> Versions { get; set; } = new List<PlayerVersion>();
    }
}