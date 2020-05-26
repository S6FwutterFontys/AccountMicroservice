using System;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using AccountMicroservice.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AccountMicroservice.Helpers
{
    public class JWTTokenGenerator : IJWTTokenGenerator
    {
        private readonly AppSettings _appSettings;

        public JWTTokenGenerator(IOptions<AppSettings> appsettings)
        {
            _appSettings = appsettings.Value;
        }

        public string GenerateJWT(Guid id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.SecretJWT);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}