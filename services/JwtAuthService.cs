
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace backend.services
{

    public class JwtAuthService
    {
        private readonly string _key;

        public JwtAuthService(string key)
        {
            _key = key;
        }

        //public string GenerateToken(string username)
        //{
        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.Name, username)
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: "your-issuer",
        //        audience: "your-audience",
        //        claims: claims,
        //        expires: DateTime.Now.AddMinutes(30),
        //        signingCredentials: creds);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}


        public string GenerateToken(string username, string userId)
        {
            // Add claims, including the NameIdentifier claim
            var claims = new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, userId) // Include the user ID as a claim
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "your-issuer",
                audience: "your-audience",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

}
