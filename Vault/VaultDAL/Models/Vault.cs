using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VaultDAL.Concrete;

namespace VaultDAL.Models
{
    public class Vault:Entity
    {
        [Required(ErrorMessage = "Field Name can't be empty")]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }
        public VaultUser VaultAdmin { get; set; }
        public IEnumerable<VaultUser> VaultUsers { get; set; }
        public IEnumerable<VaultUser> AllowRead { get; set; } 
        public IEnumerable<VaultUser> AllowCreate { get; set; } 
    }
}
