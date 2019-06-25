using StorageAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.ModelsVM
{
    public class CatalogVM : BaseEntity
    {
        /// <summary>
        /// Constructor for model
        /// </summary>
        public CatalogVM()
        {
            // Initializing our product list with empty value to avoid problems related with null references
            Products = new List<Product>();
        }
        /// <summary>
        /// Catalog name
        /// </summary>
        public string Name { get; set; }
        public Guid CatalogNameId { get; set; }
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
        /// Price per one product
        /// </summary>
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
    }
}
