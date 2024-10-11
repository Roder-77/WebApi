using Common.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services
{
    public class JwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<Appsettings> appsettings)
        {
            _jwtSettings = appsettings.Value.JwtSettings;
        }

        /// <summary>
        /// 產生 Token
        /// </summary>
        /// <param name="userName">使用者名稱</param>
        /// <param name="expireMinutes">幾分鐘後過期</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Jwt 相關設定未填</exception>
        public string GenerateToken(string userName, int expireMinutes = 60)
        {
            var key = _jwtSettings.Key;
            var issuer = _jwtSettings.Issuer;

            if (string.IsNullOrWhiteSpace(key) && string.IsNullOrWhiteSpace(issuer))
                throw new ArgumentException("Jwt settings is empty");

            var expireTime = DateTime.Now.AddMinutes(expireMinutes);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, expireTime.ToTimestamp().ToString()),
                // add costomize roles
            };

            var userClaimsIdentity = new ClaimsIdentity(claims);

            // Create a SymmetricSecurityKey for JWT Token signatures
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));

            // HmacSha256 MUST be larger than 128 bits, so the key can't be too short. At least 16 and more characters.
            // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // Create SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Subject = userClaimsIdentity,
                Expires = expireTime,
                SigningCredentials = signingCredentials
            };

            // Generate a JWT securityToken, than get the serialized Token result (string)
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }
    }
}
