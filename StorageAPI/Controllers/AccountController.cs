using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StorageAPI.Models;
using StorageAPI.ModelsVM;

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOneOrigin")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ApplicationSettings applicationSettings;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<ApplicationSettings> appSettings)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.applicationSettings = appSettings.Value;
        }

        // GET: api/Account
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Account/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] RegisterVM newUser)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = newUser.Email, FullName = newUser.FullName, UserName = newUser.Email };
                // Adding new user
                var addedUser = await userManager.CreateAsync(user, newUser.Password);
                if (addedUser.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                }
                else
                {
                    throw new Exception("Something went wrong");
                }          
            }
            return Ok(newUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginVM userThatWantToLogin)
        {
            var user = await userManager.FindByEmailAsync(userThatWantToLogin.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, userThatWantToLogin.Password))
            {
                var tokenDescriptor = new SecurityTokenDescriptor {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID", user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("vfvrbylhzybr18021")), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
            {
                return BadRequest(new { message = "USarname or password is incorrect." });
            }
        }





        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
