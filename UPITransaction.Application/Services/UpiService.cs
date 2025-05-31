using UPITransaction.Application.Common;
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

        // Enable or disable UPI for a user
        public async Task<BaseResponse<bool>> UpdateUpiStatusAsync(string phoneNumber, bool enable)
        {
            var user = await _userRepo.GetByPhoneNumberAsync(phoneNumber);
            if (user == null)
                return BaseResponse<bool>.FailureResponse("User not found.");

            user.IsUpiEnabled = enable;
            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveChangesAsync();

            return enable
                ? BaseResponse<bool>.SuccessResponse("UPI enabled.", true)
                : BaseResponse<bool>.SuccessResponse("UPI disabled.", false);
        }

        // Retrieve user balance
        public async Task<BaseResponse<decimal>> GetBalanceAsync(string phoneNumber)
        {
            var user = await _userRepo.GetByPhoneNumberAsync(phoneNumber);
            if (user == null)
                return BaseResponse<decimal>.FailureResponse("User not found.");

            return BaseResponse<decimal>.SuccessResponse("User balance retrieved successfully.", user.Balance);
        }

        // Add money to a user's account
        public async Task<BaseResponse<decimal>> AddMoneyAsync(string phoneNumber, decimal amount)
        {
            var user = await _userRepo.GetByPhoneNumberAsync(phoneNumber);
            if (user == null)
                return BaseResponse<decimal>.FailureResponse("User not found.");

            if (!user.IsUpiEnabled)
                return BaseResponse<decimal>.FailureResponse("User's UPI is disabled.");

            if (amount <= 0)
                return BaseResponse<decimal>.FailureResponse("Please enter a valid amount.");

            if (user.Balance + amount > MaxBalanceLimit)
                return BaseResponse<decimal>.FailureResponse($"Maximum balance limit of ₹{MaxBalanceLimit:n0} exceeded.");

            user.Balance += amount;
            await _userRepo.UpdateAsync(user);
            await _userRepo.SaveChangesAsync();

            return BaseResponse<decimal>.SuccessResponse("Amount successfully added.", user.Balance);
        }

        // Transfer money to another user
        public async Task<BaseResponse<bool>> TransferAsync(string senderPhone, string receiverPhone, decimal amount)
        {
            if (!IsValidAmount(amount))
                return BaseResponse<bool>.FailureResponse($"Invalid amount. Must be between ₹1 and ₹{MaxTransferLimit:n0}.");

            var sender = await _userRepo.GetByPhoneNumberAsync(senderPhone);
            var receiver = await _userRepo.GetByPhoneNumberAsync(receiverPhone);

            var validationResult = await ValidateUsersAndLimits(sender, receiver, amount);
            if (validationResult != null)
                return BaseResponse<bool>.FailureResponse(validationResult);

            await PerformTransfer(sender, receiver, amount);
            return BaseResponse<bool>.SuccessResponse("Transfer successful.", true);
        }

        // Check if the amount is valid
        private bool IsValidAmount(decimal amount)
        {
            return amount > 0 && amount <= MaxTransferLimit;
        }

        // Validate users, UPI status, balance, and limits
        private async Task<string?> ValidateUsersAndLimits(User sender, User receiver, decimal amount)
        {
            if (sender == null)
                return "Sender not found.";

            if (receiver == null)
                return "Receiver not found.";

            if (!sender.IsUpiEnabled)
                return "Sender has UPI disabled.";

            if (!receiver.IsUpiEnabled)
                return "Receiver has UPI disabled.";

            if (sender.Balance < amount)
                return "Insufficient balance.";

            var today = DateTime.UtcNow.Date;
            var todayTransactions = (await _txnRepo.GetTransactionsByUserIdAsync(sender.Id))
                .Where(txn => txn.Timestamp.Date == today && txn.SenderId == sender.Id)
                .ToList();

            if (todayTransactions.Sum(txn => txn.Amount) + amount > MaxDailyTransferAmount)
                return $"Transfer would exceed the daily limit of ₹{MaxDailyTransferAmount:n0}.";

            if (todayTransactions.Count >= MaxDailyTransferCount)
                return $"Maximum of {MaxDailyTransferCount} transfers allowed per day.";

            if (receiver.Balance + amount > MaxBalanceLimit)
                return $"Receiver's balance would exceed ₹{MaxBalanceLimit:n0}.";

            return null;
        }

        // Perform the transfer and update records
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
