using OutfixApi.Models;

namespace OutfixApi.Repositories;

public interface IOrderCollection
{
    Task<Order> Create(Order order);
    Task<Order?> GetById(string orderId);
    Task<Order?> GetByPreferenceId(string preferenceId);
    Task Update(Order order);
    
    Task<List<Order>> GetByUserId(string userId);
}