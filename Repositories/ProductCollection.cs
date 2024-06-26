﻿using MongoDB.Bson;
using MongoDB.Driver;
using OutfixApi.Models;

namespace OutfixApi.Repositories
{
    public class ProductCollection : IProductCollection
    { 
        internal MongoDBRepository _repository = new MongoDBRepository();

        private IMongoCollection<Product> Collection;

        public ProductCollection() {
            Collection = _repository.db.GetCollection<Product>("Products");
        }
        public async Task DeleteProduct(string id)
        {
            var filter = Builders<Product>.Filter.Eq(s => s.Id, id);
            await Collection.DeleteOneAsync(filter);
        }

        public async Task<List<Product>> GetAllProducts(int? limit, string? category, int page = 1)
        {

            if(!string.IsNullOrEmpty(category))
            {
                var filter = Builders<Product>.Filter.Eq(p => p.Category, category);

                return await Collection.Find(filter)
                    .Limit(limit > 0 ? limit : 20)
                    .Skip((page - 1) * limit)
                    .ToListAsync();
            }

            return await Collection.Find(new BsonDocument())
                 .Limit(limit)
                 .Skip((page - 1) * limit)
                 .ToListAsync();
        }

        public async Task<Product> GetProductById(string id)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            var product = await Collection.Find(filter).FirstOrDefaultAsync();
            return product;
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
    }
}
