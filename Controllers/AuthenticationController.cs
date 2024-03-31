using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OutfixApi.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using OutfixApi.Repositories;

namespace OutfixApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly string _secretKey;

        public AuthenticationController(IConfiguration config)
        {
            _secretKey = config.GetSection("settings").GetSection("secretKet").ToString();
        }

        [HttpPost]
        [Route("login")]

        public async Task<IActionResult> Login([FromBody] Login userLogin) {

            var userCollection = new UserCollection();

            var userFounded = await userCollection.GetUserByEmail(userLogin.Email);

            if (userFounded != null && userFounded.Email == userLogin.Email && userFounded.Password == userLogin.Password)
            {
                var keyBytes = Encoding.ASCII.GetBytes(_secretKey);
                var claims = new ClaimsIdentity();

                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, userFounded.Id));
                claims.AddClaim(new Claim(ClaimTypes.Email, userFounded.Email));
                claims.AddClaim(new Claim(ClaimTypes.Role, userFounded.Role));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMonths(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                string createdToken = tokenHandler.WriteToken(tokenConfig);

                return StatusCode(StatusCodes.Status200OK, new {token = createdToken});
            } else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
            }
        }
    }
}
