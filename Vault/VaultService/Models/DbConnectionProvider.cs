using System;
using System.Configuration;
using VaultDAL.Abstract;

namespace VaultService.Models
{
    public class DbConnectionProvider:IConnectionProvider
    {
        public string GetServer()
        {
            return ConfigurationManager.AppSettings["MongoServer"];
        }

        public string GetDatabase()
        {
            return ConfigurationManager.AppSettings["VaultDatabase"];
        }
    }
}
