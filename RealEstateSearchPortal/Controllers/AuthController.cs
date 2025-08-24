using Microsoft.AspNetCore.Mvc;
using RealEstateSearchPortal.DTOs;
using RealEstateSearchPortal.Services;

namespace RealEstateSearchPortal.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var token = await _authService.Register(registerDto);
            return Ok(new { Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var token = await _authService.Login(loginDto);
            return Ok(new { Token = token });
        }

    }
}
