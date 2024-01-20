using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PiratMessages.WebApi.Jwt;
using PiratMessages.WebApi.Models.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PiratMessages.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager; 
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtSettings _options;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IOptions<JwtSettings> optAccess)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _options = optAccess.Value;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            var user = new IdentityUser { UserName = registerRequest.UserName, Email = registerRequest.Email };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                claims.Add(new Claim(ClaimTypes.Email, registerRequest.Email));

                await _userManager.AddClaimsAsync(user, claims);

            }
            else return BadRequest();

            return Ok();
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(AuthenticateRequest authenticateRequest)
        {
            var user = await _userManager.FindByEmailAsync(authenticateRequest.Email);

            var result = await _signInManager.PasswordSignInAsync(user, authenticateRequest.Password, false, false);

            if (result.Succeeded)
            {
                IEnumerable<Claim> claims = await _userManager.GetClaimsAsync(user);
                var token = GetToken(user, claims);

                return Ok(token);
            }

            return BadRequest();
        }

        private string GetToken(IdentityUser user, IEnumerable<Claim> principal)
        {
            var claims = principal.ToList();

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
