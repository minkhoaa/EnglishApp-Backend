using EnglishApp.Data;
using EnglishApp.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EnglishApp.Service
{
    public class TokenGenerator : TokenRepository
    {
        private readonly IConfiguration _configuration;
        public TokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateRefreshToken()
        {
            throw new NotImplementedException();
        }

        public string GenerateToken(User user, IList<Claim>? additionalClaims = null)
        {
            // Lấy các claim mặc định của IdentityUser
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
            if (additionalClaims != null)
            {
                foreach (var c in additionalClaims)
                {
                    if (!claims.Any(cl => cl.Type == c.Type && cl.Value == c.Value))
                        claims.Add(c);
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddHours(6); 

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],        
                audience: _configuration["JWT:Audience"],   
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
