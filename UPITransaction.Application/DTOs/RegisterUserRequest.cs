namespace UPITransaction.Application.DTOs
{
    public class RegisterUserRequest : PhoneNumberRequest
    {
        public decimal InitialBalance { get; set; }
    }
}
