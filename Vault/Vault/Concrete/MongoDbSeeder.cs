using MongoDB.Driver;
using Vault.Abstract;

namespace Vault.Concrete    
{
    public class MongoDbSeeder:IDatabaseSeeder
    {
        private readonly IConnectionProvider _provider;

        public MongoDbSeeder(IConnectionProvider provider)
        {
            _provider = provider;
        }

        public void Seed()
        {
            var client = CreateMongoClient();
        }

        private void Seed(MongoClient client)
        {
                
        }

        private MongoClient CreateMongoClient()
        {
            return new MongoClient(_provider.GetServer());
        }
    }
}