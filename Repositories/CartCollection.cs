using MongoDB.Driver;
using OutfixApi.Models;

namespace OutfixApi.Repositories;

public class CartCollection : ICartCollection
{
    internal MongoDBRepository _repository = new MongoDBRepository();
    private IMongoCollection<Cart> Collection;

    public CartCollection()
    {
        Collection = _repository.db.GetCollection<Cart>("Carts");
    }

    public async Task<Cart?> GetByUserId(string userId)
    {
        return await Collection
            .Find(c => c.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<Cart> Create(string userId)
    {
        var cart = new Cart
        {
            UserId = userId,
            Items = new List<CartItem>(),
            UpdatedAt = DateTime.UtcNow
        };

        await Collection.InsertOneAsync(cart);
        return cart;
    }

    public async Task AddItem(string userId, CartItem item)
    {
        var cart = await GetByUserId(userId) ?? await Create(userId);

        var existingItem = cart.Items.FirstOrDefault(i =>
            i.ProductId == item.ProductId &&
            i.VariantId == item.VariantId
        );

        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            cart.Items.Add(item);
        }

        cart.UpdatedAt = DateTime.UtcNow;

        await Collection.ReplaceOneAsync(
            c => c.Id == cart.Id,
            cart
        );
    }

    public async Task UpdateItemQuantity(
        string userId,
        string productId,
        string variantId,
        int quantity
    )
    {
        var cart = await GetByUserId(userId);
        if (cart == null) return;

        var item = cart.Items.FirstOrDefault(i =>
            i.ProductId == productId &&
            i.VariantId == variantId
        );

        if (item == null) return;

        if (quantity <= 0)
        {
            cart.Items.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }

        cart.UpdatedAt = DateTime.UtcNow;

        await Collection.ReplaceOneAsync(
            c => c.Id == cart.Id,
            cart
        );
    }

    public async Task RemoveItem(string userId, string productId, string variantId)
    {
        var cart = await GetByUserId(userId);
        if (cart == null) return;

        cart.Items = cart.Items
            .Where(i => !(i.ProductId == productId && i.VariantId == variantId))
            .ToList();

        cart.UpdatedAt = DateTime.UtcNow;

        await Collection.ReplaceOneAsync(
            c => c.Id == cart.Id,
            cart
        );
    }

    public async Task ClearCart(string userId)
    {
        var cart = await GetByUserId(userId);
        if (cart == null) return;

        cart.Items.Clear();
        cart.UpdatedAt = DateTime.UtcNow;

        await Collection.ReplaceOneAsync(
            c => c.Id == cart.Id,
            cart
        );
    }
}