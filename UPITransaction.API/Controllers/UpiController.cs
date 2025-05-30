using Microsoft.AspNetCore.Mvc;
using UPITransaction.Application.Common;
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
            var result = await _upiService.UpdateUpiStatusAsync(phoneNumber, enable);

            return result.Contains("enabled") || result.Contains("disabled")
                ? Ok(BaseResponse<string>.SuccessResponse(result, phoneNumber))
                : NotFound(BaseResponse<string>.FailureResponse(result));
        }

        // Get current balance
        [HttpGet("balance/{phoneNumber}")]
        public async Task<IActionResult> GetBalance(string phoneNumber)
        {
            var balance = await _upiService.GetBalanceAsync(phoneNumber);
            return balance.HasValue
                ? Ok(BaseResponse<decimal>.SuccessResponse("Balance fetched", balance.Value))
                : NotFound(BaseResponse<decimal>.FailureResponse("User not found."));
        }

        // Add money 
        [HttpPut("add-money/{phoneNumber}")]
        public async Task<IActionResult> AddMoney(string phoneNumber, [FromBody] AddMoneyRequest request)
        {
            var result = await _upiService.AddMoneyAsync(phoneNumber, request.Amount);
            return result
                ? Ok(BaseResponse<string>.SuccessResponse("Money added", phoneNumber))
                : BadRequest(BaseResponse<string>.FailureResponse("Invalid user or amount."));
        }

        // Transfer money 
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            var message = await _upiService.TransferAsync(request.SenderPhoneNumber, request.ReceiverPhoneNumber, request.Amount);
            return message == "Transfer successful."
                ? Ok(BaseResponse<string>.SuccessResponse(message, request.Amount.ToString()))
                : BadRequest(BaseResponse<string>.FailureResponse(message));
        }
    }
}
