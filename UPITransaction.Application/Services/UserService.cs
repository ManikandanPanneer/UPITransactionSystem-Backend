using UPITransaction.Application.Common;
using UPITransaction.Application.DTOs;
using UPITransaction.Application.Interface;
using UPITransaction.DataAccessLayer.Entities;
using UPITransaction.DataAccessLayer.Repositories.Interface;

namespace UPITransaction.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // NewUser Registration
        public async Task<BaseResponse<UserInfoResponse>> RegisterUserAsync(string phoneNumber, decimal initialBalance)
        {
            var existingUser = await _userRepo.GetByPhoneNumberAsync(phoneNumber);
            if (existingUser != null)
                return BaseResponse<UserInfoResponse>.FailureResponse("User already exists.");


            if (initialBalance < 0 || initialBalance > 100000)
                return BaseResponse<UserInfoResponse>.FailureResponse("Invalid amount. Must be between ₹1 and ₹100000");

            var user = new User
            {
                PhoneNumber = phoneNumber,
                Balance = initialBalance,
                IsUpiEnabled = true   //By Default UPI is enabled for user
            };          

            
            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            var response = await _userRepo.GetByPhoneNumberAsync(phoneNumber);
            var userinfo = new UserInfoResponse
            {
                PhoneNumber = response.PhoneNumber,
                Balance = response.Balance,
                IsUpiEnabled = response.IsUpiEnabled
            };

            return BaseResponse<UserInfoResponse>.SuccessResponse("User registered successfully.", userinfo);
        }

        // Validating sender and reciver
        public async Task<BaseResponse<UserInfoResponse>> ValidateReceiverAsync(string senderPhone, string receiverPhone)
        {
            if (senderPhone == receiverPhone)
                return BaseResponse<UserInfoResponse>.FailureResponse("Sender and receiver must be different.");

            var sender = await _userRepo.GetByPhoneNumberAsync(senderPhone);
            if (sender == null || !sender.IsUpiEnabled)
                return BaseResponse<UserInfoResponse>.FailureResponse("Sender not valid or UPI not enabled.");

            var receiver = await _userRepo.GetByPhoneNumberAsync(receiverPhone);
            if (receiver == null || !receiver.IsUpiEnabled)
                return BaseResponse<UserInfoResponse>.FailureResponse("Receiver not valid or UPI not enabled.");

            var userinfo = new UserInfoResponse
            {
                PhoneNumber = sender.PhoneNumber,
                Balance = sender.Balance,
                IsUpiEnabled = sender.IsUpiEnabled
            };

            return BaseResponse<UserInfoResponse>.SuccessResponse("Receiver is valid.", userinfo);
        }

        //Checking valid user or not (Login)
        public async Task<BaseResponse<UserInfoResponse>> ValidateUserAsync(string phoneNumber)
        {
            var user = await _userRepo.GetByPhoneNumberAsync(phoneNumber);
            if (user == null)
                return BaseResponse<UserInfoResponse>.FailureResponse("Invalid user. Please register.");

            var userinfo = new UserInfoResponse
            {
                PhoneNumber = user.PhoneNumber,
                Balance = user.Balance,
                IsUpiEnabled = user.IsUpiEnabled
            };

            return BaseResponse<UserInfoResponse>.SuccessResponse("User is valid.", userinfo);
        }

        // Retrieves user information like balance and UPI status
        public async Task<BaseResponse<UserInfoResponse>> GetUserInfoAsync(string phoneNumber)
        {
            var user = await _userRepo.GetByPhoneNumberAsync(phoneNumber);
            if (user == null)
                return BaseResponse<UserInfoResponse>.FailureResponse("User not found.");

            var info = new UserInfoResponse
            {
                PhoneNumber = user.PhoneNumber,
                Balance = user.Balance,
                IsUpiEnabled = user.IsUpiEnabled
            };

            return BaseResponse<UserInfoResponse>.SuccessResponse("User data retrieved.", info);
        }
    }
}
