using UPITransaction.Application.Common;
using UPITransaction.Application.DTOs;

namespace UPITransaction.Application.Interface
{
    public interface IUpiService
    {
        Task<BaseResponse<bool>> UpdateUpiStatusAsync(string phoneNumber, bool enable);
        Task<BaseResponse<decimal>> GetBalanceAsync(string phoneNumber);
        Task<BaseResponse<decimal>> AddMoneyAsync(string phoneNumber, decimal amount);
        Task<BaseResponse<bool>> TransferAsync(string senderPhone, string receiverPhone, decimal amount);
    }
}
