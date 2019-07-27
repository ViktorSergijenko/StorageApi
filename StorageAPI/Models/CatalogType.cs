using System;
using System.Collections.Generic;
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
    }
}
