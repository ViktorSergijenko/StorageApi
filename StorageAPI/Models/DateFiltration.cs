﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    public class DateFiltration
    {
        public bool LastThirtyMinutes { get; set; }
        public bool LastHour { get; set; }
        public bool LastSixHours { get; set; }
        public bool LastWeek { get; set; }
        public bool LastMonth { get; set; }
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTill { get; set; }
        public Guid WarehouseId { get; set; }

    }
}
