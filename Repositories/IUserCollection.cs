using OutfixApi.Models;

namespace OutfixApi.Repositories
{
    public interface IUserCollection
    {
        Task CreateUser(User user);
        Task DeleteUser(string id);
        Task<User> GetUserById(string id);
        Task<List<User>> GetAllUsers();
        Task UpdateUser(User user);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByName(string name);
    }
}
