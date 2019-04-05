using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    public class Product : BaseEntity
    {
        /// <summary>
        /// Product name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Product price for a unit
        /// </summary>
        public decimal PricePerOne { get; set; }
        /// <summary>
        /// Vendor code(Артикул), varable that will help to distinguish same products in different warehouses
        /// </summary>
        public string VendorCode { get; set; }
        /// <summary>
        /// Catalog  that product is attached to
        /// </summary>
        public Catalog Catalog { get; set; }
        /// <summary>
        /// Id of a catalog that product is attached to
        /// </summary>
        public Guid CatalogId { get; set; }

    }
}
