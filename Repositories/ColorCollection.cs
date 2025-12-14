using MongoDB.Bson;
using MongoDB.Driver;
using OutfixApi.Models;

namespace OutfixApi.Repositories
{
    public class ColorCollection : IColorCollection
    {
        internal MongoDBRepository _repository = new MongoDBRepository();

        private IMongoCollection<Color> Collection;

        public ColorCollection()
        {
            Collection = _repository.db.GetCollection<Color>("Colors");
        }

        public async Task AddColor(Color color)
        {   
            await Collection.InsertOneAsync(color);
        }

        public async Task UpdateColor(Color color)
        {
            await Collection.ReplaceOneAsync(x => x.Id == color.Id, color);
        }

        public async Task DeleteColor(string id)
        {
            await Collection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<List<Color>> GetAllColors()
        {
            return await Collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }

        public async Task<Color> GetColorById(string id)
        {
            return await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}