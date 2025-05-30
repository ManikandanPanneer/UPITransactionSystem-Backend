using Microsoft.EntityFrameworkCore;
using UPITransaction.DataAccessLayer.Entities;
using UPITransaction.DataAccessLayer.Repositories.Interface;

namespace UPITransaction.DataAccessLayer.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly UpiDbContext _context;

        public UserRepository(UpiDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.PhoneNumber == phoneNumber);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
