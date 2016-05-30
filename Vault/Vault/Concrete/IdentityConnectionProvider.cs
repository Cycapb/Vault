﻿using System.Configuration;
using VaultDAL.Abstract;


namespace Vault.Concrete
{
    public class IdentityConnectionProvider:IConnectionProvider
    {
        public string GetServer()
        {
            return ConfigurationManager.AppSettings["MongoServer"];
        }

        public string GetDatabase()
        {
            return ConfigurationManager.AppSettings["IdentityDatabase"];
        }
    }
}