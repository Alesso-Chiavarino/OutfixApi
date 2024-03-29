using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OutfixApi.Models
{
    public interface Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
    }
}
