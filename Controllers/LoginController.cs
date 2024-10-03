using DinamisAPI.Models;
using DinamisAPI.Services;
using DinamisAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DinamisAPI.Controllers
{
    public class LoginController : ControllerBase
    {
        private readonly UserManager<DinamisUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IAuthServices _authServices;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginController(IHttpContextAccessor httpContextAccessor, UserManager<DinamisUser> userManager, IAuthServices authServices, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _authServices = authServices;
            _httpContextAccessor = httpContextAccessor;
        }


        [AllowAnonymous]
        [HttpPost("GetToken")]
        public async Task<IActionResult> GetToken([FromBody] UserViewModel request)
        {
            var response = await _authServices.Login(request);

            if (response.Success)
            {
                return Ok(new
                {
                    Status = "1",
                    Token = response.Token!.ToString(),
                    Message = "Token generated"
                });
            }
            else
            {
                return Ok(new
                {
                    Status = "2",
                    Message = response.Message!.ToString()
                });
            }
        }
    }
}
