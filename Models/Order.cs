using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OutfixApi.Models;

public class OrderItem
{
    // Referencias (opcional, solo informativas)
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; set; } = null!;

    public string VariantId { get; set; } = null!;

    // 🔒 SNAPSHOT DEL PRODUCTO
    public string Title { get; set; } = null!;
    public string Image { get; set; } = null!;
    public decimal UnitPrice { get; set; }

    // Compra
    public int Quantity { get; set; }

    // Snapshot de variante (opcional pero recomendado)
    public string Size { get; set; } = null!;
    public string ColorName { get; set; } = null!;
}

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = null!;

    public List<OrderItem> Items { get; set; } = new();

    public decimal Total { get; set; }

    // MercadoPago
    public string PreferenceId { get; set; } = null!;
    public string? PaymentId { get; set; }
    public string Status { get; set; } = "pending"; // pending | approved | rejected

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}