using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TelephoneBookAssignment.Shared.Entities
{
    public class BaseMongoDbEntity
    {
        [BsonId]
        public ObjectId ObjectId { get; set; } = ObjectId.GenerateNewId();
    }
}