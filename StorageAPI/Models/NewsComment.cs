using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    public class NewsComment : BaseEntity
    {
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public News News { get; set; }
        public Guid NewsId { get; set; }
        public string Comment { get; set; }
    }
}
