using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StorageAPI.Context;
using StorageAPI.Models;
using StorageAPI.ModelsVM;

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOneOrigin")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        protected StorageContext DB { get; private set; }

        public UserProfileController(UserManager<User> userManager, IServiceProvider service)
        {
            this.userManager = userManager;
            DB = service.GetRequiredService<StorageContext>();
        }

        [HttpGet("get-profile")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetUserProfile()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "UserID").Value;
           var role = User.Claims.FirstOrDefault(x => x.Type == "Role").Value;
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
               var userBasket = await DB.Baskets.FirstOrDefaultAsync(x => x.UserId == userId);
                return Ok(new UserWithBasketId
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    BasketId = userBasket.Id,
                    RoleName = role,
                    HasAbilityToLoad = user.HasAbilityToLoad,
                    UserId = user.Id,
                    
                }
                );
            }
            else
            {
                return BadRequest();
            }
            
        }
    }
}