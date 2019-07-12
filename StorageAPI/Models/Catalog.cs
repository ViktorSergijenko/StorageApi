using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models 
{
    /// <summary>
    /// Catalog model
    /// </summary>
    public class Catalog : BaseEntity
    {
        /// <summary>
        /// Constructor for model
        /// </summary>
        public Catalog() {
            // Initializing our product list with empty value to avoid problems related with null references
            Products = new List<Product>();
        }
        /// <summary>
        /// Catalog name
        /// </summary>
        public CatalogName Name { get; set; }
        public Guid CatalogNameId { get; set; }
        /// <summary>
        /// Catalog current product amount
        /// </summary>
        public decimal CurrentAmount { get; set; }
        /// <summary>
        /// Max stock of products that catalog can store
        /// </summary>
        [Required]
        public int MaximumAmount { get; set; }
        /// <summary>
        /// Minimum stock of products that catalog should store
        /// </summary>
        [Required]
        public int MinimumAmount { get; set; }
        /// <summary>
        /// Price per one product
        /// </summary>
        [Required]
        public decimal ProductPrice { get; set; }
        /// <summary>
        /// Warehouse to that this catalog belongs
        /// </summary>
        public Warehouse Warehouse { get; set; }
        /// <summary>
        /// Id of an warehouse to that this catalog belongs
        /// </summary>
        public Guid? WarehouseId { get; set; }
        /// <summary>
        /// Basket to that this catalog belongs
        /// </summary>
        public Basket Basket { get; set; }
        /// <summary>
        /// Id of an basket to that this catalog belongs
        /// </summary>
        public Guid? BasketId { get; set; }
        /// <summary>
        /// List of products
        /// </summary>
        public List<Product> Products { get; set; }
        [Required]
        public bool Type { get; set; }
    }
}
