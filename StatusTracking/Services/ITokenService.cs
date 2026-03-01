using StatusTracking.Models;

namespace StatusTracking.Services
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();
    }
}
