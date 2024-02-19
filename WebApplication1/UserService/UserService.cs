using MongoDB.Driver;
using System.Security.Cryptography;
namespace WebApplication1.UserService
{
    public interface IUserService
    {
        Task<User> GetUserByUsername(string userName);
        Task CreateUser(User user);
        
    }
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserService(IMongoDatabase database)
        {
            _usersCollection = database.GetCollection<User>("Users");
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _usersCollection.Find(u => u.Name == username).FirstOrDefaultAsync();
        }

        public async Task CreateUser(User user)
        {
            await _usersCollection.InsertOneAsync(user);
        }
    }
}
