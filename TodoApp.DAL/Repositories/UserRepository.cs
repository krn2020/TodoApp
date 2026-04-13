using Microsoft.EntityFrameworkCore;
using TodoApp.DAL.Data;
using TodoApp.DAL.Entities;
using TodoApp.DAL.Interfaces;

namespace TodoApp.DAL.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _dbSet.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
}
