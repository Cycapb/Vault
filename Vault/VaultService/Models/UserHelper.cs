﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultDAL.Abstract;

namespace VaultService.Models
{
    public class UserHelper
    {
        private readonly VaultHelper _vaultHelper;

        public UserHelper()
        {
            _vaultHelper = new VaultHelper();

        }


    }
}
