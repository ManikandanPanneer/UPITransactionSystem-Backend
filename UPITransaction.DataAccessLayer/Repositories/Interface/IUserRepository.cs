using UPITransaction.DataAccessLayer.Entities;

namespace UPITransaction.DataAccessLayer.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task SaveChangesAsync();
    }
}
