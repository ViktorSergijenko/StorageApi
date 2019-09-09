using StorageAPI.Context;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StorageAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using StorageAPI.Helpers;
using StorageAPI.Constants;

namespace StorageAPI.Services
{
    public class CatalogTypeService
    {
        protected StorageContext DB { get; private set; }
        private readonly UserManager<User> userManager;
        public CatalogTypeService(IServiceProvider service, UserManager<User> userManager)
        {
            DB = service.GetService<StorageContext>();
            this.userManager = userManager;

        }

        // All base crud methods are stored here
        #region Base CRUD
        /// <summary>
        /// Method gets catalog types from DB that has created a specific user
        /// </summary>
        /// <param name="userId">Id of an user that has created catalog types</param>
        /// <returns>List of catalog </returns>
        public async Task<List<CatalogType>> GetAllCatalogTypes(string userId)
        {
            var user = await DB.Users.FirstOrDefaultAsync(x => x.Id == userId);
            StorageException.ThrowNotFoundIfNull(user);
            if (await userManager.IsInRoleAsync(user, StorageConstants.Role_FourthLevel))
            {
                var userLevelThreeBoss = await DB.Users.FirstOrDefaultAsync(x => x.Id == user.ReportsTo);
                StorageException.ThrowNotFoundIfNull(userLevelThreeBoss);
                return await DB.CatalogTypeDB
                .Where(x => x.UserId == userLevelThreeBoss.ReportsTo)
                .ToListAsync();

            }
            return await DB.CatalogTypeDB
                .Where(x => x.UserId == userId || x.UserId == user.ReportsTo)
                .ToListAsync();
        }

        /// <summary>
        /// Method gets catalog type object by Id
        /// </summary>
        /// <param name="id">Id of an catalog type that we want to get</param>
        /// <returns>Catalog type object</returns>
        public async Task<CatalogType> GetCatalogTypeById(Guid id)
        {
            var catalogType = await DB.CatalogTypeDB.FirstOrDefaultAsync(x => x.Id == id);
            StorageException.ThrowNotFoundIfNull(catalogType);
            return catalogType;      
        }


        public async Task<CatalogType> SaveCatalogType(CatalogType catalogType, string userId)
        {
            // If catalog type doesn't have id, that means that it is new object and we need to add it into DB
            if (catalogType.Id == null || catalogType.Id.Equals(Guid.Empty))
            {
                // Calling method that adds new type into DB
                return await CreateNewCatalogType(catalogType, userId);
            }
            // If it has an id that means that we need to edit it
            else
            {
                // Calling method that will edit catalog type in DB
                return await EditCatalogType(catalogType, userId);
            }
        }

        /// <summary>
        /// Method that deletes catalog type from DB
        /// </summary>
        /// <param name="catalogTypeId">Catalog type id that we want to delete</param>
        /// <param name="userId">Id of an user that want to delete this catalog type</param>
        public async Task DeleteCatalogType(Guid catalogTypeId, string userId)
        {
            // Getting catalog that we want to delete
            var catalogToDelete = await DB.CatalogTypeDB.FirstOrDefaultAsync(x => x.Id == catalogTypeId);
            // Getting user that wants to delete catalog type
            var userThatEditedCatalogType = await DB.Users.FirstOrDefaultAsync(x => x.Id == userId);
            // Checking objects that we got from DB on null
            StorageException.ThrowNotFoundIfNull(catalogToDelete);
            StorageException.ThrowNotFoundIfNull(userThatEditedCatalogType);
            // Checking if catalog that is going to be deleted attached/related with him or hes boss, if it's related then we deleteit from DB
            if (catalogToDelete.UserId == userId || catalogToDelete.UserId == userThatEditedCatalogType.ReportsTo)
            {
                // If there are still catalog names in DB that are related to this catalog type, then we wont allow it to be deleted
                if (catalogToDelete.Amount != 0)
                {
                    throw new StorageException("Vēl ir katalogu nosaukumi kuri ir saistīti ar šo tīpu");
                }
                // Deleting catalog type if all is okey
                DB.CatalogTypeDB.Remove(catalogToDelete);
                await DB.SaveChangesAsync();
            }
            // If user managed some how to send delete request, that is attached to other workers, then we will send a excetion with message
            else
            {
                throw new Exception("Jūms nāv tiesības lai nodzēst katalogu tipu.");
            }
        }
        #endregion Base CRUD

        // Private methods that adds or edits catalog type
        #region Add/Edit methods
        /// <summary>
        /// Method adds new catalog type object into DB
        /// </summary>
        /// <param name="catalogType">New catalog type</param>
        /// <param name="userId">User id that created catalog type</param>
        /// <returns>New catalog type</returns>
        private async Task<CatalogType> CreateNewCatalogType(CatalogType catalogType, string userId)
        {
            // Getting user that created new catalog type from DB
            var userThatCreatedCatalogType = await DB.Users.FirstOrDefaultAsync(x => x.Id == userId);
            // If he has 'second level' role, then we just initialize UserId property with id that came with the param, since all catalog types for
            // second level users are unique
            if (await userManager.IsInRoleAsync(userThatCreatedCatalogType, StorageConstants.Role_SecondLevel))
            {
                catalogType.UserId = userId;
                await DB.CatalogTypeDB.AddAsync(catalogType);
            }
            // If not, that means that it user that created new type is level three, so we need to initialize UserId property with ReportsTo property from a user
            // that we got from DB, since his boss that created him is always level two
            else
            {
                catalogType.UserId = userThatCreatedCatalogType.ReportsTo;
                await DB.CatalogTypeDB.AddAsync(catalogType);
            }
            await DB.SaveChangesAsync();
            return catalogType;
        }

        /// <summary>
        /// Method that edits catalog type in DB
        /// </summary>
        /// <param name="catalogType">Catalog type with new values</param>
        /// <param name="userId">User that made changes</param>
        /// <returns>Edited catalog type</returns>
        private async Task<CatalogType> EditCatalogType(CatalogType catalogType, string userId)
        {

            // Getting user that edited catalog type from DB
            var userThatEditedCatalogType = await DB.Users.FirstOrDefaultAsync(x => x.Id == userId);
            // Checking if catalog that has been edited is attached/related with him or hes boss, if it's related then we update values in DB
            if (catalogType.UserId == userId || catalogType.UserId == userThatEditedCatalogType.ReportsTo)
            {
                 DB.CatalogTypeDB.Update(catalogType);
                return catalogType;
            }
            // If user managed some how change catalog type values, that is attached to other workers, then we will send a excetion with message
            else
            {
                throw new Exception("Tas nav jūsu catalogu tips.");
            }
        }
        #endregion Add/Edit methods

    }
}
