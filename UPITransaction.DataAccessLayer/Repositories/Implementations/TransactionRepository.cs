using Microsoft.EntityFrameworkCore;
using UPITransaction.DataAccessLayer.Entities;
using UPITransaction.DataAccessLayer.Repositories.Interface;

namespace UPITransaction.DataAccessLayer.Repositories.Implementations
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly UpiDbContext _context;

        public TransactionRepository(UpiDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(int userId)
        {
            return await _context.Transactions
                .Where(Transaction => Transaction.SenderId == userId || Transaction.ReceiverId == userId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
