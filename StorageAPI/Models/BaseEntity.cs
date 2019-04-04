using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StorageAPI.Models
{
    /// <summary>
    /// Base entity model, inherit this to other models 
    /// </summary>
    public class BaseEntity
    {
        #region IBaseEntity

        [Key]
        public Guid Id { get; set; }

        //public byte[] RowVersion { get; set; }

        #endregion IBaseEntity

        //#region IAuditable

        //public DateTimeOffset Created { get; set; }
        //public Guid? CreatedById { get; set; }
        ////public User CreatedBy { get; set; }
        //public DateTimeOffset Modified { get; set; }
        //public Guid? ModifiedById { get; set; }
        ////public User ModifiedBy { get; set; }

        //#endregion IAuditable

        //#region IDeletable

        //public bool IsDeleted { get; set; }

        //#endregion IDeletable

        //#region IObjectState

        //public ObjectState ObjectState { get; set; }

        //#endregion IObjectState
    }
}
