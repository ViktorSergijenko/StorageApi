using System;
using System.Collections.Generic;
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
        public string Name { get; set; }
        /// <summary>
        /// Catalog current product amount
        /// </summary>
        public int CurrentAmount { get; set; }
        /// <summary>
        /// Max stock of products that catalog can store
        /// </summary>
        public int MaximumAmount { get; set; }
        /// <summary>
        /// Minimum stock of products that catalog should store
        /// </summary>
        public int MinimumAmount { get; set; }
        /// <summary>
        /// Total price for purchased goods(products in catalog)
        /// </summary>
        public decimal PurchasePrice { get; set; }
        /// <summary>
        /// Total price for purchased goods(products in catalog)
        /// </summary>
        public decimal SoldPrice { get; set; }
        /// <summary>
        /// Difference between purchase and sold price
        /// </summary>
        public decimal DifferenceBetweenSoldAndPurchasePrice { get; set; }
        /// <summary>
        /// Warehouse to that this catalog belongs
        /// </summary>
        public Warehouse Warehouse { get; set; }
        /// <summary>
        /// Id of an warehouse to that this catalog belongs
        /// </summary>
        public Guid WarehouseId { get; set; }
        /// <summary>
        /// List of products
        /// </summary>
        public List<Product> Products { get; set; }
    }
}
