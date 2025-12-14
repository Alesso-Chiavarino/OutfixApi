using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OutfixApi.Models
{
    public class Color
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        public string Name { get; set; }      // "Rojo"
        public string Code { get; set; }      // "#FF0000" (opcional)
        public string Slug { get; set; }      // "red" → útil para URLs y filtros
    }
}