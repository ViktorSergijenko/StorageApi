using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    public class Basket : BaseEntity
    {
        public Basket()
        {
            Catalogs = new List<Catalog>();
        }

        /// <summary>
        /// List of catalogs that user have in basket
        /// </summary>
        public List<Catalog> Catalogs { get; set; }
        /// <summary>
        /// User to that belongs this backet
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Id of an user to that this basket belongs
        /// </summary>
        public string UserId { get; set; }
    }

    public class BasketWithNewProductsVM {
        public Guid BasketId { get; set; }
        public List<Product> ProductList { get; set; }
    }

    public class IAddProductsToBasket
    {
        public Guid BasketId { get; set; }
        public decimal ProductAmount { get; set; }
        public Guid CatalogId { get; set; }
        public string Name { get; set; }
    }
    public enum ProblemWithBasket
    {
        NotEnoughProductsInCatalog,
        NotEnoughProductsInBasket,
        AllIsOkey
    }
}
