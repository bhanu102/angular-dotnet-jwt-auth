using JwtDemo.API.Models;
using System.Threading.Tasks;

namespace JwtDemo.API.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByRefreshTokenAsync(string token);
        Task CreateUserAsync(User user);
        Task DeleteUserAsync(Guid id);
        Task SaveChangesAsync();
    }
}
