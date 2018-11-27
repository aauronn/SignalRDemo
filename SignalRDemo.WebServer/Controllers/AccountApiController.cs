using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SignalRDemo.WebServer.Entities;
using SignalRDemo.WebServer.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SignalRDemo.WebServer.Controllers
{
    [Route("api/account")]
    public class AccountApiController : Controller
    {
        private readonly ILogger<AccountApiController> _logger;
        private readonly SignInManager<Principal> _signInManager;
        private readonly UserManager<Principal> _userManager;
        private readonly IConfiguration _config;

        public AccountApiController(ILogger<AccountApiController> logger,
            SignInManager<Principal> signInManager,
            UserManager<Principal> userManager,
            IConfiguration config)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
        }

        [HttpPost("createToken")]
        public async Task<IActionResult> CrateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);

                if (user != null)
                {
                    // Create token
                    var claims = new[]
                    {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email), // Name of the subject
                            new Claim(JwtRegisteredClaimNames.Jti, new Guid().ToString()), // unique string for each token

                            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName), // Username of the user, mapped in the identity of the user
                            new Claim("UserIP", model.UserIPAddress),
                            new Claim("UserMachineName", model.UserMachineName)
                        };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        _config["Tokens:Issuer"],
                        _config["Tokens:Audiance"],
                        claims,
                        expires: DateTime.UtcNow.AddDays(1),
                        signingCredentials: credentials
                        );

                    var results = new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    };

                    return Created("", results);

                }

            }

            return BadRequest();
        }
    }
}
