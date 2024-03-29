using MongoDB.Bson;
using MongoDB.Driver;
using OutfixApi.Models;

namespace OutfixApi.Repositories
{
    public class CategoryCollection : ICategoryCollection
    {
        internal MongoDBRepository _repository = new MongoDBRepository();

        private IMongoCollection<Category> Collection;
        public CategoryCollection()
        {
            Collection = _repository.db.GetCollection<Category>("Categories");
        }
        public async Task DeleteCategory(string id)
        {
            var filter = Builders<Category>.Filter.Eq(s => s.Id, id);
            await Collection.DeleteOneAsync(filter);
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await Collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }

        public async Task<Category> GetCategoryById(string id)
        {
            var filter = Builders<Category>.Filter.Eq(p => p.Id, id);
            var category = await Collection.Find(filter).FirstOrDefaultAsync();
            return category;
        }

        public async Task InsertCategory(Category category)
        {
            await Collection.InsertOneAsync(category);
        }

        public async Task UpdateCategory(Category category)
        {
            var filter = Builders<Category>.Filter.Eq(s => s.Id, category.Id);
            await Collection.ReplaceOneAsync(filter, category);
        }
    }
}
