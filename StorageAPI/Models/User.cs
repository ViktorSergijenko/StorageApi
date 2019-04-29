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
    }
}
