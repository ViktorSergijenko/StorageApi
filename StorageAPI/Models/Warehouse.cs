using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    /// <summary>
    /// Warehouse model
    /// </summary>
    public class Warehouse: BaseEntity
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Warehouse() {
            // Initializing our catalog list with empty value to avoid problems related with null references
            Catalogs = new List<Catalog>();
            News = new List<News>();
            UserWarehouse = new List<UserWarehouse>();
            WarehouseLogs = new List<SimpleLogTable>();
        }
        /// <summary>
        /// Name of the warehouse
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Address where warehouse is located
        /// </summary>
        [Required]
        public string Address { get; set; }
        /// <summary>
        /// Flag that indicates does warehouse have some kind of problems
        /// </summary>
        public bool HasProblems { get; set; }
        [Required]
        public bool HasMinCatalogs { get; set; }
        /// <summary>
        /// Location name
        /// </summary>
        [Required]
        public string Location { get; set; }
        /// <summary>
        /// Variable that stores QR code as base64
        /// </summary>
        public string QrCodeBase64 { get; set; }
        /// <summary>
        /// Variable that stores warehouse image as base64
        /// </summary>
        public string ImageBase64 { get; set; }
        /// <summary>
        /// Warehouse type flag, that indicates is it a main warehouse or not
        /// </summary>
        [Required]
        public WarehouseType Type { get; set; }
        /// <summary>
        /// List of catalogs with products that are stored in warehouse
        /// </summary>
        public List<Catalog> Catalogs { get; set; }
        public List<News> News { get; set; }
        public List<UserWarehouse> UserWarehouse { get; set; }
        public List<SimpleLogTable> WarehouseLogs { get; set; }

    }

    /// <summary>
    /// Enum that is used as a flag to indicate warehouse type
    /// </summary>
    public enum WarehouseType {
        MainWarehouse,
        SimpleWarehouse
    }
}
