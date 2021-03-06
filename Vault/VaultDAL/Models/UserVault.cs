﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VaultDAL.Concrete;

namespace VaultDAL.Models
{
    [CustomTimeComparator]
    public class UserVault:Entity
    {
        [Required(ErrorMessage = "Field Name can't be empty")]
        public string Name { get; set; }
        public string Description { get; set; }
        public int OpenTime { get; set; }
        public int CloseTime { get; set; }
        public VaultUser VaultAdmin { get; set; }
        public IList<VaultUser> AllowRead { get; set; } 
        public IList<VaultUser> AllowCreate { get; set; }
        public IList<string> VaultItems { get; set; } 
    }
}
