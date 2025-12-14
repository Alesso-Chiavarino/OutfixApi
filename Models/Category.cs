using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OutfixApi.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }       

        public string? Description { get; set; }

        public string? Icon { get; set; }     

        public bool Active { get; set; } = true;

        public string? ParentId { get; set; }  
    }
}
