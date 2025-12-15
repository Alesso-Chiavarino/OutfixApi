using MongoDB.Driver;
using OutfixApi.Models;

namespace OutfixApi.Repositories;

public class OrderCollection : IOrderCollection
{
    private readonly IMongoCollection<Order> Collection;

    public OrderCollection()
    {
        var repository = new MongoDBRepository();
        Collection = repository.db.GetCollection<Order>("Orders");
    }

    public async Task<Order> Create(Order order)
    {
        await Collection.InsertOneAsync(order);
        return order;
    }

    public async Task<Order?> GetById(string orderId)
    {
        return await Collection
            .Find(o => o.Id == orderId)
            .FirstOrDefaultAsync();
    }

    public async Task<Order?> GetByPreferenceId(string preferenceId)
    {
        return await Collection
            .Find(o => o.PreferenceId == preferenceId)
            .FirstOrDefaultAsync();
    }

    public async Task Update(Order order)
    {
        order.UpdatedAt = DateTime.UtcNow;

        await Collection.ReplaceOneAsync(
            o => o.Id == order.Id,
            order
        );
    }
    
    public async Task<List<Order>> GetByUserId(string userId)
    {
        return await Collection
            .Find(o => o.UserId == userId)
            .SortByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
}