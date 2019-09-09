using StorageAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.ModelsVM
{
    public class WarehouseVM: BaseEntity
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
        /// Flag that indicates does warehouse have some kind of problems
        /// </summary>
        public bool HasProblems { get; set; }
        public bool HasMinCatalogs { get; set; }

        public string WarehouseLogs { get; set; }
        public int WarehousePositionInTable { get; set; }
    }
}
