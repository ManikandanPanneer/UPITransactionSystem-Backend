using Microsoft.AspNetCore.Mvc;
using UPITransaction.Application.DTOs;
using UPITransaction.Application.Interface;

namespace UPITransaction.API.Controllers
{
    [ApiController]
    [Route("api/upi")]
    public class UpiController : ControllerBase
    {
        private readonly IUpiService _upiService;

        public UpiController(IUpiService upiService)
        {
            _upiService = upiService;
        }

        // Enable or disable UPI
        [HttpPatch("upi-status/{phoneNumber}")]
        public async Task<IActionResult> UpdateUpiStatus(string phoneNumber, [FromBody] bool enable)
        {
            var response = await _upiService.UpdateUpiStatusAsync(phoneNumber, enable);

            return response.Success ? Ok(response) : NotFound(response);
        }


        // Get current balance
        [HttpGet("balance/{phoneNumber}")]
        public async Task<IActionResult> GetBalance(string phoneNumber)
        {
            var response = await _upiService.GetBalanceAsync(phoneNumber);

            return response.Success ? Ok(response) : NotFound(response);
        }

        // Add money
        [HttpPut("add-money/{phoneNumber}")]
        public async Task<IActionResult> AddMoney(string phoneNumber, [FromBody] AddMoneyRequest request)
        {
            var response = await _upiService.AddMoneyAsync(phoneNumber, request.Amount);

            return response.Success ? Ok(response) : NotFound(response);
        }

        // Transfer money 
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            var response = await _upiService.TransferAsync(request.SenderPhoneNumber, request.ReceiverPhoneNumber, request.Amount);

            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}
