using Microsoft.AspNetCore.Mvc;
using UPITransaction.Application.DTOs;
using UPITransaction.Application.Interface;

namespace UPITransaction.API.Controllers
{
    // Handles user-related operations
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Register a new user
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var response = await _userService.RegisterUserAsync(request.PhoneNumber, request.InitialBalance);
            if (!response.Success)
                return Conflict(response); // User already exists

            return Ok(response);
        }

        // Get user info by phone number
        [HttpGet("{phoneNumber}")]
        public async Task<IActionResult> GetUserInfo(string phoneNumber)
        {
            var response = await _userService.GetUserInfoAsync(phoneNumber);
            return response.Success ? Ok(response) : NotFound(response);
        }

        // Validate if user exists
        [HttpGet("validate/{phoneNumber}")]
        public async Task<IActionResult> ValidateUser(string phoneNumber)
        {
            var response = await _userService.ValidateUserAsync(phoneNumber);
            return response.Success ? Ok(response) : NotFound(response);
        }

        // Validate receiver before transfer
        [HttpGet("validate-receiver")]
        public async Task<IActionResult> ValidateReceiver([FromQuery] string senderPhone, [FromQuery] string receiverPhone)
        {
            var response = await _userService.ValidateReceiverAsync(senderPhone, receiverPhone);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
