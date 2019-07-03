using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    public class AdminLogTable : BaseEntity
    {
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Where { get; set; }
        public string What { get; set; }
        public DateTime Date { get; set; }
    }
}
