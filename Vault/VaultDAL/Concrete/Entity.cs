using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using VaultDAL.Abstract;

namespace VaultDAL.Concrete
{
    public abstract class Entity : IEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
