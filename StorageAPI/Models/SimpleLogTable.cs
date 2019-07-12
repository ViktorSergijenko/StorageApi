using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    public class SimpleLogTable : BaseEntity
    {
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Where { get; set; }
        public string What { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Warehouse Warehouse { get; set; }
        public Guid WarehouseId { get; set; }
        public string Manually { get; set; }


    }
}
