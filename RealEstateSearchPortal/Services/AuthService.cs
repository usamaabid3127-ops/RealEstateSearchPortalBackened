namespace RealEstateSearchPortal.Services
{
    using Microsoft.EntityFrameworkCore;
    using RealEstateSearchPortal.Data;
    using RealEstateSearchPortal.DTOs;
    using RealEstateSearchPortal.Models;

    namespace RealEstatePortal.Services
    {
        public class AuthService : IAuthService
        {
            private readonly AppDbContext _context;
            private readonly IJwtService _jwtService;

            public AuthService(AppDbContext context, IJwtService jwtService)
            {
                _context = context;
                _jwtService = jwtService;
            }

            public async Task<string> Register(RegisterDto registerDto)
            {
                if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                {
                    throw new Exception("User already exists");
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
                var user = new User
                {
                    Email = registerDto.Email,
                    PasswordHash = passwordHash
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return _jwtService.GenerateToken(user.Id);
            }

            public async Task<string> Login(LoginDto loginDto)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                if (user == null)
                {
                    throw new Exception("Email Does not exists");
                }
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    throw new Exception("Incorrect password please re-try");
                }
                return _jwtService.GenerateToken(user.Id);
            }
        }
    }
}
