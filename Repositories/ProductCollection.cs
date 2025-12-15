using MongoDB.Bson;
using MongoDB.Driver;
using OutfixApi.Models;

namespace OutfixApi.Repositories
{
    public class ProductCollection : IProductCollection
    {
        internal MongoDBRepository _repository = new MongoDBRepository();
        private IMongoCollection<Product> Collection;

        public ProductCollection()
        {
            Collection = _repository.db.GetCollection<Product>("Products");
        }

        public async Task DeleteProduct(string id)
        {
            var filter = Builders<Product>.Filter.Eq(s => s.Id, id);
            await Collection.DeleteOneAsync(filter);
        }

        public async Task<List<Product>> GetAllProducts(
            int? limit,
            string? category,
            string? search,
            int page = 1
        )
        {
            var filters = new List<FilterDefinition<Product>>();

            if (!string.IsNullOrEmpty(category))
                filters.Add(Builders<Product>.Filter.Eq(p => p.Category, category));

            if (!string.IsNullOrEmpty(search))
                filters.Add(
                    Builders<Product>.Filter.Regex(
                        p => p.Title,
                        new BsonRegularExpression(search, "i")
                    )
                );

            var filter = filters.Count > 0
                ? Builders<Product>.Filter.And(filters)
                : new BsonDocument();

            return await Collection.Find(filter)
                .Limit(limit > 0 ? limit : 20)
                .Skip((page - 1) * (limit ?? 20))
                .ToListAsync();
        }

        public async Task<List<Product>> GetUserProducts(
            string owner,
            int? limit,
            string? category,
            int page = 1
        )
        {
            var filters = new List<FilterDefinition<Product>>
            {
                Builders<Product>.Filter.Eq(p => p.Owner, owner)
            };

            if (!string.IsNullOrEmpty(category))
                filters.Add(Builders<Product>.Filter.Eq(p => p.Category, category));

            var filter = Builders<Product>.Filter.And(filters);

            int pageSize = limit ?? 20;
            int skip = (page - 1) * pageSize;

            return await Collection.Find(filter)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<List<Product>> GetAllOwnedProducts(
            string owner,
            int? limit,
            string? category,
            int page = 1)
        {
            var filters = new List<FilterDefinition<Product>>
            {
                Builders<Product>.Filter.Eq(p => p.Owner, owner)
            };

            if (!string.IsNullOrEmpty(category))
                filters.Add(Builders<Product>.Filter.Eq(p => p.Category, category));

            var filter = Builders<Product>.Filter.And(filters);

            int pageSize = limit ?? 20;
            int skip = (page - 1) * pageSize;

            return await Collection.Find(filter)
                .Skip(skip)
                .Limit(pageSize)
                .SortByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<Product> GetProductById(string id)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task InsertProduct(Product product)
        {
            await Collection.InsertOneAsync(product);
        }

        public async Task UpdateProduct(Product product)
        {
            var filter = Builders<Product>.Filter.Eq(s => s.Id, product.Id);
            await Collection.ReplaceOneAsync(filter, product);
        }

        // ✅ NUEVO MÉTODO: RESTAR STOCK POR VARIANTE
        public async Task DecreaseStock(string productId, string variantId, int quantity)
        {
            // variantId = "S_693af93d4c215b7e422efe90"
            var parts = variantId.Split('_');
            if (parts.Length != 2)
                return;

            var size = parts[0];
            var colorId = parts[1];

            var filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(p => p.Id, productId),
                Builders<Product>.Filter.ElemMatch(
                    p => p.Variants,
                    v => v.Size == size && v.Color == colorId
                )
            );

            var update = Builders<Product>.Update.Inc(
                "Variants.$.Stock",
                -quantity
            );

            await Collection.UpdateOneAsync(filter, update);
        }
    }
}