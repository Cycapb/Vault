using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using VaultDAL.Abstract;

namespace VaultDAL.Concrete
{
    public class MongoRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IConnectionProvider provider)
        {
            _mongoClient = new MongoClient(provider.GetServer());
            _mongoDatabase = _mongoClient.GetDatabase(provider.GetDatabase());
            _collection = _mongoDatabase.GetCollection<T>(GetCollectionNameFromType(typeof (T)));
        }

        private string GetCollectionNameFromType(Type entitytype)
        {
            return entitytype.Name.ToLower();
        }

        public async Task<IEnumerable<T>> GetListAsync()
        {
            return await _collection.AsQueryable().ToListAsync();
        }

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                return await _collection.AsQueryable().Where(x => x.Id == id).SingleOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<T> CreateAsync(T item)
        {
            await _collection.InsertOneAsync(item);
            return item;
        }

        public async Task DeleteAsync(string id)
        {
            var filter = new BsonDocument("_id", new BsonObjectId(new ObjectId(id)));
            await _collection.FindOneAndDeleteAsync(filter);
        }

        public async Task UpdateAsync(T item)
        {
            var filter = new BsonDocument("_id", new BsonObjectId(new ObjectId(item.Id)));
            await _collection.ReplaceOneAsync(filter, item);
        }

        public void Dispose()
        {
            
        }
    }
}
