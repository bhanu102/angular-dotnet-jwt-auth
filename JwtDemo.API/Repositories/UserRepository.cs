using JwtDemo.API.Data;
using JwtDemo.API.Interfaces;
using JwtDemo.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace JwtDemo.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.Include(u => u.RefreshTokens)
                                 .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Include(u => u.RefreshTokens)
                                 .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.Include(u => u.RefreshTokens)
                                 .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string token)
        {
            return await _context.Users.Include(u => u.RefreshTokens)
                                 .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == token));
        }

        public async Task CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await GetUserByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await SaveChangesAsync();
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
