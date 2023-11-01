using EmployeeManagerAPI.Helpers;
using EmployeeManagerAPI.Infrastructure.Helpers;
using EmployeeManagerAPI.Infrastructure.Interfaces;
using EmployeeManagerAPI.Infrastructure.Models;
using EmployeeManagerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static EmployeeManagerAPI.Infrastructure.Models.Database;

namespace EmployeeManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IUserDataProvider _dataProvider;
        private readonly IOptions<JwtSettings> _jwtSettings;

        public AccountController(IUserDataProvider dataProvider, IOptions<JwtSettings> jwtSettings)
        {
            _dataProvider = dataProvider;
            _jwtSettings = jwtSettings;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(Login login)
        {
            // get response
            APIResponse<User> _response = new APIResponse<User>();

            if (!ModelState.IsValid)
                return BadRequest(new APIResponse<User> { Status = false, Msg = "Invalid model!" });

            try
            {
                var passwordHasher = new PasswordHasher<User>();

                // get USER
                User user = await _dataProvider.GetUser(new wpsp_User_Select
                {
                    UserName = login.UserName
                });
                if (user == null)
                {
                    _response = new APIResponse<User>() { Status = false, Msg = "User does not exist!" };
                    return Ok(_response);
                }

                // verify user
                var passwordVerificationResult = passwordHasher.VerifyHashedPassword(null, user.PasswordHash, login.Password);
                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    _response = new APIResponse<User>() { Status = false, Msg = "Invalid credentials!" };
                    return Ok(_response);
                }

                // generate TOKEN
                string token = GenerateToken(user);
                _response = new APIResponse<User>() { Status = true, Msg = token, Value = user };
                return Ok(_response);
            }
            catch
            {
                _response = new APIResponse<User>() { Status = false, Msg = "Could not login." };
                return StatusCode(500, _response);
            }
        }

        [NonAction]
        [AllowAnonymous]
        private string GenerateToken(User user)
        {
            // get laims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.RoleName)
            };

            // generate a symmetric security key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Secret));

            // generate signing credentials using the security key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            // create a JWT token
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Value.Issuer,
                audience: _jwtSettings.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            // serialize the token to a string
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult> RefreshToken()
        {
            // retrieve the existing token from the request
            string existingToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // decode the existing token
            var existingTokenHandler = new JwtSecurityTokenHandler();
            var existingTokenDecoded = existingTokenHandler.ReadJwtToken(existingToken);

            // get the user info
            var userId = existingTokenDecoded.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userRole = existingTokenDecoded.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // get the user
            var user = await _dataProvider.GetUser(new wpsp_User_Select { UserId = int.Parse(userId) });

            if (user != null)
            {
                // generate a new access token for the user
                string newToken = GenerateToken(user);

                return Ok(new APIResponse<string>() { Status = true, Msg = "Ok", Value = newToken });
            }
            else
            {
                return StatusCode(500, new APIResponse<string>() { Status = true, Msg = "Invalid token!" });
            }
        }
    }
}
