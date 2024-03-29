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

        public async Task<Category> CreateCategory(Category category)
        {
            await Collection.InsertOneAsync(category);
            return category;
        }

        public async Task<Category> DeleteCategory(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Category>> GetCategories()
        {
            return await Collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }

        public async Task<Category> GetCategoryById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Category> UpdateCategory(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
