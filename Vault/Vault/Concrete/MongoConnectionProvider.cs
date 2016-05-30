using System.Configuration;
using Vault.Abstract;

namespace Vault.Concrete
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