using JwtDemo.API.Dtos;
using JwtDemo.API.Interfaces;
using JwtDemo.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IGoogleAuthService _googleAuthService;

        public UsersController(IUserRepository userRepository, ITokenService tokenService, IGoogleAuthService googleAuthService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _googleAuthService = googleAuthService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] UserRegisterDto request)
        {
            if (await _userRepository.GetUserByEmailAsync(request.Email) != null)
                return BadRequest(new AuthResponseDto { Success = false, Errors = new List<string> { "User already exists" } });

            if (await _userRepository.GetUserByUsernameAsync(request.Username) != null)
                return BadRequest(new AuthResponseDto { Success = false, Errors = new List<string> { "User already exists" } });

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password)
            };

            await _userRepository.CreateUserAsync(user);

            return Ok(new AuthResponseDto { Success = true });
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromBody] UserLoginDto request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized(new AuthResponseDto { Success = false, Errors = new List<string> { "Invalid credentials" } });

            var jwtToken = _tokenService.GenerateJwtToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Setup Refresh Token
            refreshToken.UserId = user.Id;
            user.RefreshTokens.Add(refreshToken);
            await _userRepository.SaveChangesAsync();

            return Ok(new AuthResponseDto 
            { 
                Success = true, 
                Token = jwtToken, 
                RefreshToken = refreshToken.Token 
            });
        }

        [HttpPost("google-signin")]
        public async Task<IActionResult> GoogleSignin([FromBody] GoogleAuthDto request)
        {
            try
            {
                var payload = await _googleAuthService.VerifyGoogleToken(request.IdToken);
                var user = await _userRepository.GetUserByEmailAsync(payload.Email);

                if (user == null)
                {
                    // Create new user if not exists
                    user = new User
                    {
                        Email = payload.Email,
                        Username = payload.GivenName ?? payload.Email.Split('@')[0],
                        PasswordHash = "" // No password for Google users
                    };
                    await _userRepository.CreateUserAsync(user);
                }

                var jwtToken = _tokenService.GenerateJwtToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                refreshToken.UserId = user.Id;
                user.RefreshTokens.Add(refreshToken);
                await _userRepository.SaveChangesAsync();

                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Token = jwtToken,
                    RefreshToken = refreshToken.Token
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new AuthResponseDto { Success = false, Errors = new List<string> { "Google Auth failed: " + ex.Message } });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto request)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(request.RefreshToken);

            if (user == null)
                return BadRequest(new AuthResponseDto { Success = false, Errors = new List<string> { "Invalid Refresh Token" } });

            var storedToken = user.RefreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);

            if (storedToken == null || !storedToken.IsActive)
                return BadRequest(new AuthResponseDto { Success = false, Errors = new List<string> { "Invalid or Expired Refresh Token" } });

            // Revoke current refresh token (Rotation)
            storedToken.Revoked = DateTime.UtcNow;

            // Generate new pair
            var jwtToken = _tokenService.GenerateJwtToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            
            newRefreshToken.UserId = user.Id;
            user.RefreshTokens.Add(newRefreshToken);
            
            await _userRepository.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                Success = true,
                Token = jwtToken,
                RefreshToken = newRefreshToken.Token
            });
        }

        [Authorize]
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null)
                return Unauthorized(new AuthResponseDto { Success = false, Errors = new List<string> { "User not authenticated" } });

            await _userRepository.DeleteUserAsync(Guid.Parse(userId));

            return Ok(new AuthResponseDto { Success = true });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(hash)) return false; // Google users have no password
            return HashPassword(password) == hash;
        }
    }
}
