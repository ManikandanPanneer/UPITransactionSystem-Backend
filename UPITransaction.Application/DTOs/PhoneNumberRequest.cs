using System.ComponentModel.DataAnnotations;

namespace UPITransaction.Application.DTOs
{
    public class PhoneNumberRequest
    {
        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }
    }
}
