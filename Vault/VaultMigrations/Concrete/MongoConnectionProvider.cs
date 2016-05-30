using System.Configuration;
using VaultMigrations.Abstract;

namespace VaultMigrations.Concrete
{
    public class MongoConnectionProvider:IConnectionProvider
    {
        public string GetServer()
        {
            return ConfigurationManager.AppSettings["MongoServer"];
        }

        public string GetDatabase()
        {
            return ConfigurationManager.AppSettings["Database"];
        }
    }
}