namespace UPITransaction.Application.Interface
{
    public interface IUpiService
    {
        Task<string> UpdateUpiStatusAsync(string phoneNumber, bool enable);
        Task<decimal?> GetBalanceAsync(string phoneNumber);
        Task<bool> AddMoneyAsync(string phoneNumber, decimal amount);
        Task<string> TransferAsync(string senderPhone, string receiverPhone, decimal amount);
    }
}
