using MongoDB.Bson.Serialization.Attributes;

namespace VaultDAL.Abstract
{
    public interface IEntity
    {
        [BsonId]
        string Id { get; set; }
    }
}
