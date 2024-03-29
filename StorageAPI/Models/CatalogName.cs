﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public string Name { get; set; }
        public int Amount { get; set; }
        public List<Catalog> CatalogList { get; set; }
        public CatalogType CatalogType { get; set; }
        public Guid? CatalogTypeId { get; set; }
        public User UserThatCreated { get; set; }
        public string UserId { get; set; }
    }
}
