using System;
using System.Collections.Generic;
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
        }
        /// <summary>
        /// Name of the warehouse
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Address where warehouse is located
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Flag that indicates does warehouse have some kind of problems
        /// </summary>
        public bool HasProblems { get; set; }
        /// <summary>
        /// Location name
        /// </summary>
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
        public WarehouseType Type { get; set; }
        /// <summary>
        /// List of catalogs with products that are stored in warehouse
        /// </summary>
        public List<Catalog> Catalogs { get; set; }
        public List<News> News { get; set; }
    }

    /// <summary>
    /// Enum that is used as a flag to indicate warehouse type
    /// </summary>
    public enum WarehouseType {
        MainWarehouse,
        SimpleWarehouse
    }
}
