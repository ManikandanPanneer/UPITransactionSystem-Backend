namespace UPITransaction.Application.DTOs
{
    public class UserInfoResponse
    {
        public string PhoneNumber { get; set; }
        public decimal Balance { get; set; }
        public bool IsUpiEnabled { get; set; }
    }
}
