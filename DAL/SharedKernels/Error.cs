using System.Net;

namespace DAL.SharedKernels
{
    public record Error(string Title, HttpStatusCode StatusCode, object? Details)
    {
        public static readonly Error None = new("No Error", HttpStatusCode.OK, null);
    }
}
