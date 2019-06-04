using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    public class User : IdentityUser
    {
        /// <summary>
        /// User full name
        /// </summary>
        public string FullName { get; set; }
        public Basket Basket { get; set; }

    }

    public class UserWithBasketId
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public Guid BasketId { get; set; }
    }
}
