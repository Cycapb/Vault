using System;
using VaultDAL.Concrete;

namespace VaultDAL.Models
{
    public class VaultAccessLog:Entity
    {
        public string Event { get; set; }
        public string EventType { get; set; }
        public DateTime DateTime { get; set; }
        public string VaultId { get; set; }
    }
}
