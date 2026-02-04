using JwtDemo.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JwtDemo.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public ProfileController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _userRepository.GetUserByIdAsync(Guid.Parse(userId));
            if (user == null) return NotFound("User not found");

            return Ok(new
            {
                user.Username,
                user.Email,
                MemberSince = user.MemberSince.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                CreatedAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Message = "This is a protected endpoint data."
            });
        }
    }
}
