using MongoDB.Bson;
using MongoDB.Driver;
using OutfixApi.Models;

namespace OutfixApi.Repositories
{
    public class UserCollection : IUserCollection
    {
        internal MongoDBRepository _repository = new MongoDBRepository();

        private IMongoCollection<User> Collection;
        public UserCollection() {
            Collection = _repository.db.GetCollection<User>("Users");
        }
        public async Task CreateUser(User user)
        {
            await Collection.InsertOneAsync(user);
        }

        public async Task DeleteUser(string id)
        {
            var filter = Builders<User>.Filter.Eq(s => s.Id, id);
            await Collection.DeleteOneAsync(filter);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await Collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }

        public async Task<User> GetUserById(string id)
        {
            var filter = Builders<User>.Filter.Eq(p => p.Id, id);
            var user = await Collection.Find(filter).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var filter = Builders<User>.Filter.Eq(p => p.Email, email);
            var user = await Collection.Find(filter).FirstOrDefaultAsync();
            return user;
        }

        public async Task UpdateUser(User user)
        {
            var filter = Builders<User>.Filter.Eq(s => s.Id, user.Id);
            await Collection.ReplaceOneAsync(filter, user);
        }
    }
}
