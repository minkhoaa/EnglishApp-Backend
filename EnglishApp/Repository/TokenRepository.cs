using EnglishApp.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Policy;

namespace EnglishApp.Repository
{
    public interface TokenRepository
    {
        public string GenerateToken(User user, IList<Claim>? additionalClaims = null);
        public string GenerateRefreshToken();
    }
}
