using UPITransaction.Application.Common;
using UPITransaction.Application.DTOs;
using UPITransaction.DataAccessLayer.Entities;

namespace UPITransaction.Application.Interface
{
    public interface IUserService
    {
        Task<BaseResponse<UserInfoResponse>> RegisterUserAsync(string phoneNumber, decimal initialBalance);
        Task<BaseResponse<bool>> ValidateReceiverAsync(string senderPhone, string receiverPhone);
        Task<BaseResponse<UserInfoResponse>> ValidateUserAsync(string phoneNumber);
        Task<BaseResponse<UserInfoResponse>> GetUserInfoAsync(string phoneNumber);
    }
}
