using UPITransaction.Application.Interface;
using UPITransaction.DataAccessLayer.Entities;
using UPITransaction.DataAccessLayer.Repositories.Interface;

namespace UPITransaction.Application.Services
{
    public class UpiService : IUpiService
    {
        private readonly IUserRepository _userRepo;
        private readonly ITransactionRepository _txnRepo;

        // Business Rule Constants
        private const decimal MaxBalanceLimit = 100000m;
        private const decimal MaxTransferLimit = 20000m;
        private const decimal MaxDailyTransferAmount = 50000m;
        private const int MaxDailyTransferCount = 3;

        public UpiService(IUserRepository userRepo, ITransactionRepository txnRepo)
        {
            _userRepo = userRepo;
            _txnRepo = txnRepo;
        }

        //UPI Status change
        public async Task<string> UpdateUpiStatusAsync(string phoneNumber, bool enable)
        {
            var user = await _userRepo.GetByPhoneNumberAsync(phoneNumber);
            if (user == null) return "User not found.";

            user.IsUpiEnabled = enable;
            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveChangesAsync();

            return enable ? "UPI enabled" : "UPI disabled";
        }


        // Getting balance
        public async Task<decimal?> GetBalanceAsync(string phoneNumber)
        {
            var user = await _userRepo.GetByPhoneNumberAsync(phoneNumber);
            return user?.Balance;
        }


        // Adding Money to users account
        public async Task<bool> AddMoneyAsync(string phoneNumber, decimal amount)
        {
            var user = await _userRepo.GetByPhoneNumberAsync(phoneNumber);
            if (user == null || !user.IsUpiEnabled || amount <= 0)
                return false;

            // Maximum balance checking
            if (user.Balance + amount > MaxBalanceLimit)
                return false;

            user.Balance += amount;
            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveChangesAsync();
            return true;
        }

        // Transfers money to another user
        public async Task<string> TransferAsync(string senderPhone, string receiverPhone, decimal amount)
        {

            if (!IsValidAmount(amount))
                return $"Invalid amount. Must be between ₹1 and ₹{MaxTransferLimit:n0}.";

            var sender = await _userRepo.GetByPhoneNumberAsync(senderPhone);
            var receiver = await _userRepo.GetByPhoneNumberAsync(receiverPhone);

            // validating the users
            var validationResult = await ValidateUsersAndLimits(sender, receiver, amount);
            if (validationResult != null)
                return validationResult;

            // Transfer Process
            await PerformTransfer(sender, receiver, amount);
            return "Transfer successful.";
        }

        // Checking amount is valid or not
        private bool IsValidAmount(decimal amount)
        {
            return amount > 0 && amount <= MaxTransferLimit;
        }

        // Validates user, UPI status, balance, limit
        private async Task<string?> ValidateUsersAndLimits(User sender, User receiver, decimal amount)
        {
            if (sender == null || receiver == null)
                return "Sender or receiver not found.";

            if (!sender.IsUpiEnabled || !receiver.IsUpiEnabled)
                return "UPI not enabled for sender or receiver.";

            if (sender.Balance < amount)
                return "Insufficient balance.";

            var today = DateTime.UtcNow.Date;
            var todayTransactions = (await _txnRepo.GetTransactionsByUserIdAsync(sender.Id))
                .Where(transations => transations.Timestamp.Date == today && transations.SenderId == sender.Id)
                .ToList();


            //Transaction amount limit validation
            //1 user can send max 50000 per day (even sending for different user)
            if (todayTransactions.Sum(transations => transations.Amount) + amount > MaxDailyTransferAmount)
                return $"Transfer would exceed daily limit of ₹{MaxDailyTransferAmount:n0}.";



            // Transaction count validation
            // 1 user can make 3 payment per day not depends between the user
            if (todayTransactions.Count >= MaxDailyTransferCount)
                return $"Maximum {MaxDailyTransferCount} transfers allowed per day.";

            if (receiver.Balance + amount > MaxBalanceLimit)
                return $"Receiver's balance exceeds ₹{MaxBalanceLimit:n0}.";

            return null;
        }


        // Transfer Process
        private async Task PerformTransfer(User sender, User receiver, decimal amount)
        {
            sender.Balance -= amount;
            receiver.Balance += amount;

            var transaction = new Transaction
            {
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                Amount = amount,
                Timestamp = DateTime.UtcNow
            };

            await _userRepo.UpdateAsync(sender);
            await _userRepo.UpdateAsync(receiver);
            await _txnRepo.AddAsync(transaction);
            await _txnRepo.SaveChangesAsync();
        }
    }
}
