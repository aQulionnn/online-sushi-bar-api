using DAL.SharedKernels;
using System.Net;

namespace DAL.Errors
{
    public static class MenuItemErrors
    {
        public static Error ValidationError(object details) => new Error("One or more validation errors occurred.", HttpStatusCode.BadRequest, details);

        public static readonly Error NotFound = new Error("Not found.", HttpStatusCode.NotFound, null);

        public static readonly Error Failure = new Error("Something went wrong.", HttpStatusCode.InternalServerError, null);
    }
}
