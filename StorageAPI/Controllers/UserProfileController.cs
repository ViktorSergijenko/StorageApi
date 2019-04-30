using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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


        public UserProfileController(UserManager<User> userManager)
        {
            this.userManager = userManager;

        }

        [HttpGet("get-profile")]
        public async Task<Object> GetUserProfile()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await userManager.FindByEmailAsync(userId);
            return new {
                user.FullName,
                user.Email,
            };
        }
    }
}