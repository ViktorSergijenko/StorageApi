using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    public class UserWarehouse
    {
        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public bool DoesUserHaveAbilityToSeeProductAmount { get; set; }
        public int WarehousePositionInTable { get; set; }
    }
}
