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
        public DateTime Date { get; set; }
    }
}
