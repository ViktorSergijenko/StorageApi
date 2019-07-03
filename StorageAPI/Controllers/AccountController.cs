using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StorageAPI.Configs;
using StorageAPI.Context;
using StorageAPI.Models;
using StorageAPI.ModelsVM;
using StorageAPI.Services;

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOneOrigin")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<User> signInManager;
        private readonly ApplicationSettings applicationSettings;
        private SimpleLogTableServcie SimpleLogTableService { get; set; }

        protected StorageContext DB { get; private set; }

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<ApplicationSettings> appSettings, IServiceProvider service, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.applicationSettings = appSettings.Value;
            SimpleLogTableService = service.GetRequiredService<SimpleLogTableServcie>();
            DB = service.GetRequiredService<StorageContext>();
        }

        // GET: api/Account
        [HttpGet]
        public async Task<string> Get()
        {
           var isThereAnyUserInDb = await DB.Users.AnyAsync();
            var allUsers = await userManager.Users.ToListAsync();
            if (!isThereAnyUserInDb)
            {
                await roleManager.CreateAsync(new IdentityRole("Level one"));
                await roleManager.CreateAsync(new IdentityRole("Level two"));
                await roleManager.CreateAsync(new IdentityRole("Level three"));
                await roleManager.CreateAsync(new IdentityRole("Level four"));
                var roleForAdmin = await roleManager.FindByNameAsync("Level one");
                RegisterVM registerInfo = new RegisterVM {
                    Email = "root@root.com",
                    FullName = "Admin Admin",
                    Password = "P@ssw0rd",
                    PasswordConfirm = "P@ssw0rd",
                    HasAbilityToLoad = true
                };
                User newUser = new User { Email = registerInfo.Email, FullName = registerInfo.FullName, UserName = registerInfo.Email, HasAbilityToLoad = true};
                var addedUser = await userManager.CreateAsync(newUser, registerInfo.Password);
                if (addedUser.Succeeded)
                {
                    await signInManager.SignInAsync(newUser, false);
                    Basket newBasket = new Basket()
                    {
                        UserId = newUser.Id
                    };
                    await DB.Baskets.AddAsync(newBasket);
                    UserSettings userSettings = new UserSettings() {
                        CanAddProductsManually = true,
                        CanDeleteUsers = true,
                        CanEditUserBaskets = true,
                        CanEditUserInformation = true,
                        CanEditUserPassword = true,
                        UserId = newUser.Id
                    };
                   await DB.UserSettingsDB.AddAsync(userSettings);


                    await userManager.AddToRoleAsync(newUser, "Level one");
                    await DB.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Something went wrong");
                }
            }
            return "asd";
        }

        // GET: api/Account/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            
            return "value";
        }
       
        [HttpGet("getUserList")]
        public async Task<ActionResult> GetUserList()
        {
            var userList = await userManager.Users.ToListAsync();
            List<UserVM> userListWithRoles = new List<UserVM>();
            foreach (var user in userList)
            {
                var userRoleName = await userManager.GetRolesAsync(user);
                UserVM userWithRole = new UserVM {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    RoleName = userRoleName[0],
                    HasAbilityToLoad = user.HasAbilityToLoad
                    
                };
                userListWithRoles.Add(userWithRole);
            }
            return Ok(userListWithRoles);
        }

        [HttpGet("getRoleList")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetRoleList()
        {
            List<string> roles = new List<string>();
            var role = User.Claims.FirstOrDefault(x => x.Type == "Role").Value;
            if (role == "Level one")
            {
                roles = await roleManager.Roles.Where(x => x.Name != "Level one").Select(x => x.Name).ToListAsync();
            }
            if (role == "Level two")
            {
                roles = await roleManager.Roles.Where(x => x.Name !="Level one" && x.Name != "Level two").Select(x => x.Name).ToListAsync();
            }
            if (role == "Level three")
            {
                roles = await roleManager.Roles.Where(x => x.Name != "Level one" && x.Name != "Level two" && x.Name != "Level three").Select(x => x.Name).ToListAsync();
            }
            return Ok(roles);
        }
      
        [HttpGet("getUserBasket")]
        public async Task<ActionResult> GetUserBasket(User user)
        {
            var userBasket = await DB.Baskets.FirstOrDefaultAsync(x => x.UserId == user.Id);
            return Ok(userBasket);
        }

     
        [HttpPost("register")]
        [Authorize(AuthenticationSchemes = "Bearer")]

        public async Task<ActionResult> RegisterUser([FromBody] RegisterVM newUser)
        {
            var whoCreated = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;

            if (ModelState.IsValid)
            {
                User user = new User { Email = newUser.Email, FullName = newUser.FullName, UserName = newUser.Email, WhoCreated = whoCreated };
                if (newUser.RoleName != "Level four")
                {
                    user.HasAbilityToLoad = true;
                }
                
                // Adding new user
                var addedUser = await userManager.CreateAsync(user, newUser.Password);
                if (addedUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, newUser.RoleName);
                    await signInManager.SignInAsync(user, false);
                    Basket newBasket = new Basket()
                    {
                        UserId = user.Id
                    };
                    DB.Baskets.Add(newBasket);
                    await DB.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Something went wrong");
                }          
            }
            await SimpleLogTableService.AddAdminLog($"Reģistrēja jauno darbnieku: { newUser.FullName}", whoCreated);

            return Ok(newUser);
        }


        [HttpPost("delete")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Delete(string id)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;

            User user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
            }
            await SimpleLogTableService.AddAdminLog($"Nodzesa darbnieku: {user.FullName}", username);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginVM userThatWantToLogin)
        {
            var user = await userManager.FindByEmailAsync(userThatWantToLogin.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, userThatWantToLogin.Password))
            {
                var userRole = await  userManager.GetRolesAsync(user);
                //var tokenDescriptor = new SecurityTokenDescriptor {
                //    Subject = new ClaimsIdentity(new Claim[]
                //    {
                //        new Claim("UserID", user.Id.ToString())
                //    }),
                //    Expires = DateTime.UtcNow.AddDays(5),
                //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("vfvrbylhzybr18021")), SecurityAlgorithms.HmacSha256Signature)
                //};
                //var tokenHandler = new JwtSecurityTokenHandler();
                //var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                //var token = tokenHandler.WriteToken(securityToken);
                //return Ok(new { token });
                var claims = new List<Claim>
                {
                    new Claim("UserID", user.Id.ToString()),
                    new Claim("UserName", user.UserName),
                    new Claim("FullName", user.FullName),
                    new Claim("Role", userRole[0]),
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                var now = DateTime.UtcNow;
                // создаем JWT-токен
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        claims: claimsIdentity.Claims,
                        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    username = claimsIdentity.Name
                };

                return Ok(response);


            }
            else
            {
                return BadRequest(new { message = "Username or password is incorrect." });
            }
        }

        [HttpPost("changeUserPassword")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;
            if (ModelState.IsValid)
            {
                User user = await userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    var _passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                    IdentityResult result =
                        await _passwordValidator.ValidateAsync(userManager, user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                        await userManager.UpdateAsync(user);
                        await SimpleLogTableService.AddAdminLog($"Izmainīja parole darbniekam: {user.FullName}", username);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }

            return Ok(model);
        }

        [HttpPost("edit-user")]
        public async Task<IActionResult> Edit(ChangeUserInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.FullName = model.FullName;
                    user.HasAbilityToLoad = model.HasAbilityToLoad;
                    var newRole = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name == model.RoleName);
                    var result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, newRole.Name);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return Ok(model);
        }
    }
}
