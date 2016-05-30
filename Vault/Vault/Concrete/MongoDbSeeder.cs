using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
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

        public async void Seed()
        {
            var client = CreateMongoClient();
            var list = await GetDatabases(client);
            var exist = IsExists(list, _provider.GetDatabase());
        }

        private void Seed(MongoClient client)
        {
                
        }

        private bool IsExists(IEnumerable<BsonDocument> dbList,string database)
        {
            var exist = false;
            foreach (var db in dbList)
            {
                var values = db.Values.AsEnumerable().ToList();
                if (values.Any(x => x.ToString() == database))
                {
                    exist = true;
                    break;
                }
            }
            return exist;
        }

        private async Task<IEnumerable<BsonDocument>> GetDatabases(MongoClient client)
        {
            var databasesList = await client.ListDatabasesAsync();
            var list = await databasesList.ToListAsync();
            return list;
        }

        private MongoClient CreateMongoClient()
        {
            return new MongoClient(_provider.GetServer());
        }
    }
}