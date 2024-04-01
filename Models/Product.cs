using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OutfixApi.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string[] Images {  get; set; }
        public int Stock { get; set; }
        public double Price { get; set; }
    }
}
