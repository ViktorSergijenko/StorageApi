using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    /// <summary>
    /// Class that describes CatalogType DB table
    /// </summary>
    public class CatalogType : BaseEntity
    {
        public CatalogType()
        {
            CatalogNameList = new List<CatalogName>();
        }
        /// <summary>
        /// Catalog type name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// List of catalog names with this kind of catalog type
        /// </summary>
        public List<CatalogName> CatalogNameList { get; set; }
        /// <summary>
        /// User that created this type
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Id of an user that created this type(Used to created relationship between user and CatalogType tables)
        /// </summary>
        [Required]
        public string UserId { get; set; }
        /// <summary>
        /// Amount of catalog names with this type
        /// </summary>
        public int Amount { get; set; }

    }
}
