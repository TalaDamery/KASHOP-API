using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAuthenicationService _authenicationService;

        public AccountsController(IAuthenicationService authenicationService) {
            _authenicationService = authenicationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request) {
            var result = await _authenicationService.RegisterAsync(request);
            if (result.Success == false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authenicationService.LoginAsync(request);
            if(result.Success == false) {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var isConfimed = await _authenicationService.ConfirmEmailAsync(token, userId);
            if(isConfimed) 
                return Ok();

            return BadRequest(); 
        }

        [HttpPost("sendCode")]
        public async Task<IActionResult> RequestpasswordResetAsync(ForgotPasswordRequest request)
        {
            var result= await _authenicationService.RequestPasswordResetAsync(request);
            if(result.Success == false) return BadRequest(result);
            return Ok(result);

        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> PasswordReset(ResetPasswordRequest request)
        {
            var result = await _authenicationService.ResetPasswordAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
