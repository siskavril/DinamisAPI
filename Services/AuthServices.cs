using DinamisAPI.Models;
using DinamisAPI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DinamisAPI.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<DinamisUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthServices(UserManager<DinamisUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<ResponseAPI> Login(UserViewModel request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName!);

            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password!))
            {
                string token = await GenerateToken(user);

                return new ResponseAPI
                {
                    Success = true,
                    Message = "Token Successfully Generated",
                    Token = token
                };
            }
            else if (user == null)
            {
                return new ResponseAPI { Message = "Username " + user + " not found, please contact your Administrator." };
            }
            else
            {
                return new ResponseAPI { Message = "Wrong Password" };
            }
        }

        private async Task<string> GenerateToken(DinamisUser user)
        {
            var dataUser = await _userManager.FindByNameAsync(user.UserName!);
            var userRoles = await _userManager.GetRolesAsync(dataUser!);

            var authClaim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            foreach (var roles in userRoles)
            {
                authClaim.Add(new Claim(ClaimTypes.Role, roles));
            }

            int expMinutes = Convert.ToInt32(_configuration["JWT:ExpiredMinutes"]);
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Secret").Value!));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var expiresToken = DateTime.Now.AddMinutes(expMinutes);
            var jwtToken = new JwtSecurityToken
                            (
                                issuer: _configuration["JWT:ValidIssuer"],
                                audience: _configuration["JWT:ValidAudience"],
                                expires: DateTime.Now.AddMinutes(expMinutes),
                                claims: authClaim,
                                signingCredentials: credential
                            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return token;
        }

    }
}
