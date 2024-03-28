using MongoDB.Driver;

namespace OutfixApi.Repositories
{
    public class MongoDBRepository
    {
        public MongoClient client;

        public IMongoDatabase db;

        public MongoDBRepository()
        {
            client = new MongoClient("mongodb+srv://alesso-chiavarino:TheAlexOMG14@outfixcluster.svkflym.mongodb.net/?retryWrites=true&w=majority&appName=OutfixCluster");

            db = client.GetDatabase("Outfix");
        }
    }
}
