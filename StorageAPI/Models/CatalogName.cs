using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    public class CatalogName : BaseEntity
    {
        public CatalogName()
        {
            CatalogList = new List<Catalog>();
        }
        public string Name { get; set; }
        public int Amount { get; set; }
        public List<Catalog> CatalogList { get; set; }
    }
}
