using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    public class News : BaseEntity
    {
        public News()
        {
            NewsComments = new List<NewsComment>();
        }
        [Required]
        public string Title { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public List<NewsComment> NewsComments { get; set; }
        public bool FixedProblem { get; set; }
        public bool IsDeleted { get; set; }
        public string Author { get; set; }
        public bool AcceptedFix { get; set; }
        public string AuthorAcceptedFix { get; set; }


        public DateTime CreatedDate { get; set; }

        public DateTime? FixedDate { get; set; }
    }
    public class NewsResolveDTO
    {
        public Guid Id { get; set; }
        public string AuthorAcceptedFix { get; set; }

    }
}
