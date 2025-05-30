namespace UPITransaction.Application.DTOs
{
    public class AddMoneyRequest : PhoneNumberRequest
    {
        public decimal Amount { get; set; }
    }
}
