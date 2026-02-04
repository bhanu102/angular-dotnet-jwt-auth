using Google.Apis.Auth;
using System.Threading.Tasks;

namespace JwtDemo.API.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken);
    }
}
