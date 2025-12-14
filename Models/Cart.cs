using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace OutfixApi.Models;

public class CartItem
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; set; } = null!;

    // Variante concreta del producto
    public string VariantId { get; set; } = null!;

    public int Quantity { get; set; }
}

public class Cart
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = null!;

    public List<CartItem> Items { get; set; } = new();

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class CartItemDetail
{
    public string ProductId { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public decimal Price { get; set; }

    public string Size { get; set; }
    public Color Color { get; set; }

    public int Quantity { get; set; }
}

public class CartDetailResponse
{
    public string CartId { get; set; }
    public List<CartItemDetail> Items { get; set; }
    public decimal Total { get; set; }
}