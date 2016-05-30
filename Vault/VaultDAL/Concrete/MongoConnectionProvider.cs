using System.Configuration;
using VaultDAL.Abstract;

namespace VaultDAL.Concrete
{
    public class MongoConnectionProvider:IConnectionProvider
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
