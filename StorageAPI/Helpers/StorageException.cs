using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace StorageAPI.Helpers
{
    public class StorageException : Exception
    {
        #region Error shortcuts

        public static StorageException NotFound { get { return new StorageException("NotFound", StatusCodes.Status404NotFound); } }
        public static StorageException BadRequest { get { return new StorageException("BadRequest", StatusCodes.Status400BadRequest); } }
        public static StorageException Forbidden { get { return new StorageException("Forbidden", StatusCodes.Status403Forbidden); } }
        public static StorageException UpdateConcurrency { get { return new StorageException("TheDatabaseRecordHasBeenChangedBySomeone", StatusCodes.Status409Conflict); } }
        public static StorageException InternalServerError { get { return new StorageException("UndefinedInternalServerError", StatusCodes.Status500InternalServerError); } }

        #endregion Error shortcuts
        public StorageException(string message, int code = StatusCodes.Status400BadRequest)
            : base(message)
        {
            Code = code;
        }
        public int Code { get; set; }

        #region Helpers

        /// <summary>
        /// If object is null - throw 404 not found error
        /// </summary>
        /// <param name="entity">Object, which is being inspected</param>
        public static void ThrowNotFoundIfNull(object entity)
        {
            if (entity == null) { throw NotFound; }
        }

        /// <summary>
        /// If object is null - throw 400 bad request error
        /// </summary>
        /// <param name="entity">Object, which is being inspected</param>
        public static void ThrowBadRequestIfNull(object entity)
        {
            if (entity == null) { throw BadRequest; }
        }

        #endregion Helpers
        public class ValidationException : Exception
        {
            public int Code { get; set; }

            public ValidationException(ModelStateDictionary modelState, int code = StatusCodes.Status400BadRequest)
                // Not sure if this works fine. Will see later.
                : base(JsonConvert.SerializeObject(modelState.Values))
            {
                Code = code;
            }
        }

        public class HMGuruExceptionVM
        {
            public string Message { get; set; }
        }
    }
}
