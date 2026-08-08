using Microsoft.AspNetCore.Mvc.Filters;

namespace DatabaseMastery.TransportMongoDb.Filters
{
    public sealed class AuthenticatedNoStoreFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated != true)
            {
                return;
            }

            var headers = context.HttpContext.Response.Headers;
            headers["Cache-Control"] = "no-store, no-cache, max-age=0";
            headers["Pragma"] = "no-cache";
            headers["Expires"] = "0";
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
