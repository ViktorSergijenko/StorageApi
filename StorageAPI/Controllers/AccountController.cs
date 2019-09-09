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
using StorageAPI.Constants;
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
                RegisterVM registerInfo = new RegisterVM
                {
                    Email = "root@root.com",
                    FullName = "Admin Admin",
                    Password = "P@ssw0rd",
                    PasswordConfirm = "P@ssw0rd",
                    HasAbilityToLoad = true
                };
                User newUser = new User { Email = registerInfo.Email, FullName = registerInfo.FullName, UserName = registerInfo.Email, HasAbilityToLoad = true };
                var addedUser = await userManager.CreateAsync(newUser, registerInfo.Password);
                if (addedUser.Succeeded)
                {
                    await signInManager.SignInAsync(newUser, false);
                    Basket newBasket = new Basket()
                    {
                        UserId = newUser.Id
                    };
                    await DB.Baskets.AddAsync(newBasket);
                    UserSettings userSettings = new UserSettings()
                    {
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetUserList()
        {
            var role = User.Claims.FirstOrDefault(x => x.Type == "Role").Value;
            var userIdThatWantToRecieveUsers = User.Claims.FirstOrDefault(x => x.Type == "UserID").Value;

            var userThatWantToRecieveUsers = await DB.Users.FirstOrDefaultAsync(x => x.Id == userIdThatWantToRecieveUsers);

                var userRole = await DB.Roles.FirstOrDefaultAsync(x => x.Name == role);
                var firstLevel = await DB.Roles.FirstOrDefaultAsync(x => x.Name == "Level one");
                var levelTwo = await DB.Roles.FirstOrDefaultAsync(x => x.Name == "Level two");
                var levelThree = await DB.Roles.FirstOrDefaultAsync(x => x.Name == "Level three");

            List<string> userIds = new List<string>();
                if (role == "Level one")
                {
                userIds =  await DB.Users.Select(x => x.Id).ToListAsync();
                }
                if (role == "Level two")
                {
                // Getting users that serves to him, level three users
                    userIds = await DB.Users
                    .Where(x => x.ReportsTo == userThatWantToRecieveUsers.Id)
                    .Select(x => x.Id)
                    .Distinct()
                    .ToListAsync()
                    ;
                // Getting all level four users that serves to level three
                var employeesOfEmployees = await DB.Users.Where(x => userIds.Any(y => y == x.ReportsTo)).Select(x => x.Id).ToListAsync();

                employeesOfEmployees.ForEach(x => userIds.Add(x));
                }
            if (role == "Level three")
            {
                // Getting all users that serves to this employee
                userIds = await DB.Users
                    .Where(x => x.ReportsTo == userThatWantToRecieveUsers.Id)
                    .Select(x => x.Id)
                    .Distinct().
                    ToListAsync()
                    ;

                // Getting his boss(level two who created him)
                var bossId = await DB.Users
                    .Where(x => x.Id == userThatWantToRecieveUsers.ReportsTo)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync()
                    ;

                // Getting all level three users that his boss has created
               var thisBossAllLevelThreeEmployeesId = await DB.Users
                   .Where(x => x.ReportsTo == bossId)
                   .Select(x => x.Id)
                   .Distinct().
                   ToListAsync()
                   ;

                // Getting all those level three user employees too
                var employeesOfEmployees = await DB.Users.Where(x => thisBossAllLevelThreeEmployeesId.Any(y => y == x.ReportsTo)).Select(x => x.Id).ToListAsync();
                // Adding boss to the list
                userIds.Add(bossId);
                // Adding level three users
                thisBossAllLevelThreeEmployeesId.ForEach(x => userIds.Add(x));
                // Adding level four users that serves to level three
                employeesOfEmployees.ForEach(x => userIds.Add(x));
            }
            var userList = await DB.Users.Where(x => userIds.Any(y => y == x.Id)).ToListAsync();
                List<UserVM> userListWithRoles = new List<UserVM>();
                foreach (var user in userList)
                {
                    var userRoleName = await userManager.GetRolesAsync(user);
                    UserVM userWithRole = new UserVM
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        RoleName = userRoleName[0],
                        HasAbilityToLoad = user.HasAbilityToLoad,
                        ReportsTo = user.ReportsTo
                    };
                    userListWithRoles.Add(userWithRole);
                }
                return Ok(userListWithRoles);
        }

        [HttpPost("get-employees-that-dont-have-access")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetMyEmployeesForWarehouse(UserWarehouse userWarehouse)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type == "Role").Value;
            var idOfAnUser = User.Claims.FirstOrDefault(x => x.Type == "UserID").Value;

            List<UserVM> usersVM = new List<UserVM>();

            if (role == "Level one")
            {
                var usersForLoop = await DB.Users.Where(x => x.Id != userWarehouse.UserId).ToListAsync();

                foreach (var user in usersForLoop)
                {
                    if (!await DB.UserWarehouseDB.AnyAsync(x => x.UserId == user.Id && x.WarehouseId == userWarehouse.WarehouseId))
                    {
                        var userRoleName = await userManager.GetRolesAsync(user);
                        UserVM userWithRole = new UserVM
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            Email = user.Email,
                            RoleName = userRoleName[0],
                            HasAbilityToLoad = user.HasAbilityToLoad,
                            ReportsTo = user.ReportsTo
                        };
                        usersVM.Add(userWithRole);
                    }
                }
            }

            if (role == "Level two")
            {
                var allEmployees = new List<User>();
                var levelThreeEmployees = await DB.Users.Where(x => x.ReportsTo == userWarehouse.UserId).ToListAsync();
                allEmployees.AddRange(levelThreeEmployees);
                var employeesFirstLoop = await DB.Users.Where(x => x.ReportsTo == userWarehouse.UserId).ToListAsync();
                foreach (var employee in levelThreeEmployees)
                {
                    if (await DB.Users.AnyAsync(x => x.ReportsTo == employee.Id))
                    {
                        allEmployees.AddRange(await DB.Users.Where(x => x.ReportsTo == employee.Id).ToListAsync());
                    }
                }
                var listOnlyWithAllowedUsers = new List<User>();
                foreach (var user in allEmployees)
                {
                    if (!await DB.UserWarehouseDB.AnyAsync(x => x.UserId == user.Id && x.WarehouseId == userWarehouse.WarehouseId))
                    {
                        listOnlyWithAllowedUsers.Add(user);
                        var userRoleName = await userManager.GetRolesAsync(user);
                        UserVM userWithRole = new UserVM
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            Email = user.Email,
                            RoleName = userRoleName[0],
                            HasAbilityToLoad = user.HasAbilityToLoad,
                            ReportsTo = user.ReportsTo
                        };
                        usersVM.Add(userWithRole);
                    }
                }

            }

            if (role == "Level three")
            {
                var levelFourEmployees = await DB.Users.Where(x => x.ReportsTo == userWarehouse.UserId).ToListAsync();
                var listOnlyWithAllowedUsers = new List<User>();
                listOnlyWithAllowedUsers.AddRange(levelFourEmployees);
                foreach (var user in levelFourEmployees)
                {
                    if (!await DB.UserWarehouseDB.AnyAsync(x => x.UserId == user.Id && x.WarehouseId == userWarehouse.WarehouseId))
                    {
                        var userRoleName = await userManager.GetRolesAsync(user);
                        UserVM userWithRole = new UserVM
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            Email = user.Email,
                            RoleName = userRoleName[0],
                            HasAbilityToLoad = user.HasAbilityToLoad,
                            ReportsTo = user.ReportsTo
                        };
                        usersVM.Add(userWithRole);
                    }
                }

            }
            return Ok(usersVM);
        }

        [HttpGet("getRoleList")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetRoleList()
        {
            var role = User.Claims.FirstOrDefault(x => x.Type == "Role").Value;
            var userIdThatWantToRecieveUsers = User.Claims.FirstOrDefault(x => x.Type == "UserID").Value;
            var userThatWantToRecieveRoles = await DB.Users.Include(x => x.Employees).FirstOrDefaultAsync(x => x.Id == userIdThatWantToRecieveUsers);
            List<string> roles = new List<string>();
            if (role == "Level one")
            {
                    roles = await roleManager.Roles.Where(x => x.Name == "Level two").Select(x => x.Name).ToListAsync();
                //if (userThatWantToRecieveRoles.Employees.Any())
                //{
                //    if ()
                //    {

                //    }
                //}
                //else
                //{
                //}
            }
            if (role == "Level two")
            {
                //roles = await roleManager.Roles.Where(x => x.Name !="Level one" && x.Name != "Level two").Select(x => x.Name).ToListAsync();
                roles = await roleManager.Roles.Where(x => x.Name == "Level three").Select(x => x.Name).ToListAsync();
            }
            if (role == "Level three")
            {
                roles = await roleManager.Roles.Where(x => x.Name == "Level four").Select(x => x.Name).ToListAsync();
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
            var creator = await DB.Users.FirstOrDefaultAsync(x => x.FullName == whoCreated);

            if (ModelState.IsValid)
            {
                User user = new User { Email = newUser.Email, FullName = newUser.FullName, UserName = newUser.Email, WhoCreated = whoCreated, ReportsTo = creator.Id };
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
        public async Task<ActionResult> Delete([FromBody]UserVM userVM)
        {
            var username = User.Claims.FirstOrDefault(x => x.Type == "FullName").Value;

            User user = await userManager.FindByIdAsync(userVM.Id);
            if (user != null)
            {
                var basket = await DB.Baskets.Include(x => x.Catalogs).FirstOrDefaultAsync(x => x.UserId == user.Id);
                var role = await userManager.GetRolesAsync(user);
                if (role[0] == "Level one")
                {
                    throw new Exception("You cant delete global admin");

                }
                if (basket.Catalogs.Count == 0)
                {
                    IdentityResult result = await userManager.DeleteAsync(user);
                }
                else
                {
                    throw new Exception("User basket is not empty");
                }
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
                    var userRoles = await DB.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();
                    userRoles.ForEach(x => DB.UserRoles.Remove(x));
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
        [HttpGet("second-role-users")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetAllUsersWithSecondRole()
        {
           var usersWithLevelTwoRole = await userManager.GetUsersInRoleAsync(StorageConstants.Role_SecondLevel);
            var secondRoleId = await roleManager.FindByNameAsync(StorageConstants.Role_SecondLevel);

            List<UserVM> userVMs = new List<UserVM>();
            foreach (var x in usersWithLevelTwoRole)
            {
                UserVM userWithRole = new UserVM
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    RoleName = secondRoleId.Name,
                    HasAbilityToLoad = x.HasAbilityToLoad,
                    ReportsTo = x.ReportsTo
                };
                userVMs.Add(userWithRole);
            }
            return Ok(userVMs);
        }
    }
}
