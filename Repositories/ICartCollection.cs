using OutfixApi.Models;

namespace OutfixApi.Repositories;

public interface ICartCollection
{
    Task<Cart?> GetByUserId(string userId);
    Task<Cart> Create(string userId);
    Task AddItem(string userId, CartItem item);
    Task UpdateItemQuantity(string userId, string productId, string variantId, int quantity);
    Task RemoveItem(string userId, string productId, string variantId);
    Task ClearCart(string userId);
}