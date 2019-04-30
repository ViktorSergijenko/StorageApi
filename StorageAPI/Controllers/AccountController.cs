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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StorageAPI.Context;
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
        protected StorageContext DB { get; private set; }

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<ApplicationSettings> appSettings, IServiceProvider service)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.applicationSettings = appSettings.Value;
            DB = service.GetRequiredService<StorageContext>();
        }

        // GET: api/Account
        [HttpGet]
        public async Task Get()
        {
           var isThereAnyUserInDb = await DB.Users.AnyAsync();
            if (!isThereAnyUserInDb)
            {
                RegisterVM registerInfo = new RegisterVM {
                    Email = "root@root.com",
                    FullName = "Admin Admin",
                    Password = "P@ssw0rd",
                    PasswordConfirm = "P@ssw0rd"
                };
                User newUser = new User { Email = registerInfo.Email, FullName = registerInfo.FullName, UserName = registerInfo.Email};
                var addedUser = await userManager.CreateAsync(newUser, registerInfo.Password);
                if (addedUser.Succeeded)
                {
                    await signInManager.SignInAsync(newUser, false);
                }
                else
                {
                    throw new Exception("Something went wrong");
                }
            }
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
