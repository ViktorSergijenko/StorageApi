using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    public class ApplicationSettings
    {
        public string JWT_KEY { get; set; }
        public string APP_URL { get; set; }
    }
}
