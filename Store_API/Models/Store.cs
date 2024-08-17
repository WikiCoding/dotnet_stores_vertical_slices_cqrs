using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Store_API.Models
{
    public class Store
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

    }
}
