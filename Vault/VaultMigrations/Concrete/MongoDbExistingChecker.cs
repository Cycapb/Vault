using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using VaultMigrations.Abstract;

namespace VaultMigrations.Concrete
{
    public class MongoDbExistingChecker:IExistingChecker
    {
        private readonly IConnectionProvider _provider;

        public MongoDbExistingChecker(IConnectionProvider provider)
        {
            _provider = provider;
        }

        public async Task<bool> Exist(string dbName)
        {
            var client = CreateMongoClient();
            var list = await GetDatabases(client);
            return IsExists(list, _provider.GetDatabase());
        }

        private bool IsExists(IEnumerable<BsonDocument> dbList, string database)
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

        private async Task<IEnumerable<BsonDocument>> GetDatabases(IMongoClient client)
        {
            var databasesList = client.ListDatabasesAsync().Result;
            var list = await databasesList.ToListAsync();
            return list;
        }

        private IMongoClient CreateMongoClient()
        {
            return new MongoClient(_provider.GetServer());
        }
    }
}