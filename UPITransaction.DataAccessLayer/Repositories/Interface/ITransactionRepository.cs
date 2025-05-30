using UPITransaction.DataAccessLayer.Entities;

namespace UPITransaction.DataAccessLayer.Repositories.Interface
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(int userId);
        Task SaveChangesAsync();
    }
}
