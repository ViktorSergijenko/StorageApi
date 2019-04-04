using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    public class Warehouse: BaseEntity
    {
        /// <summary>
        /// Name of the warehouse
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Address where warehouse is located
        /// </summary>
        public string Address { get; set; }
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
    }

    public enum WarehouseType {
        MainWarehouse,
        SimpleWarehouse
    }
}
