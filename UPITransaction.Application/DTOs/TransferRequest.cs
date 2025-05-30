namespace UPITransaction.Application.DTOs
{
    public class TransferRequest
    {
        public string SenderPhoneNumber { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
