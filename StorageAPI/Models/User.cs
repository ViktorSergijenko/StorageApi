using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            CatalogNames = new List<CatalogName>();
            UserWarehouse = new List<UserWarehouse>();
            Employees = new List<User>();
            CatalogTypes = new List<CatalogType>();
        }
        /// <summary>
        /// User full name
        /// </summary>
        [Required]
        public string FullName { get; set; }
        public Basket Basket { get; set; }
        public UserSettings Settings { get; set; }
        public string WhoCreated { get; set; }
        public List<UserWarehouse> UserWarehouse { get; set; }
        public bool HasAbilityToLoad { get; set; }
        public string ReportsTo { get; set; }
        /// <summary>
        /// Boss to that this employee is attached to
        /// </summary>
        public User Boss { get; set; }
        /// <summary>
        /// List of employees to that this employee is marked as a boss
        /// </summary>
        public List<User> Employees { get; set; }
        /// <summary>
        /// List of Catalog names that he created
        /// </summary>
        public List<CatalogName> CatalogNames { get; set; }
        /// <summary>
        /// List of catalog types that he created
        /// </summary>
        public List<CatalogType> CatalogTypes { get; set; }



    }

    public class UserSettings : BaseEntity
    {
        public User User { get; set; }
        public string UserId { get; set; }
        public bool CanDeleteUsers { get; set; }
        public bool CanEditUserPassword { get; set; }
        public bool CanAddProductsManually { get; set; }
        public bool CanEditUserBaskets { get; set; }
        public bool CanEditUserInformation { get; set; }
    }

    public class UserVM
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string FullName { get; set; }
        public bool HasAbilityToLoad { get; set; }
        public string ReportsTo { get; set; }
        public bool DoesUserHaveAbilityToSeeProductAmount { get; set; }
    }

    public class UserWithBasketId
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Guid BasketId { get; set; }
        public string RoleName { get; set; }
        public bool HasAbilityToLoad { get; set; }

    }
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangeUserInfoViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public bool HasAbilityToLoad { get; set; }

    }
}
