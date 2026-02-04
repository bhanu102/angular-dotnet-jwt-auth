using Google.Apis.Auth;
using JwtDemo.API.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace JwtDemo.API.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;

        public GoogleAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _configuration["Authentication:Google:ClientId"] }
            };
            
            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
    }
}
