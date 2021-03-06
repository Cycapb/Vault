﻿using System.ComponentModel.DataAnnotations;
using VaultDAL.Concrete;


namespace VaultDAL.Models
{
    public class VaultItem:Entity
    {
        [Required(ErrorMessage = "Field Name can't be empty")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Field Content can't be empty")]
        public string Content { get; set; }

    }
}
