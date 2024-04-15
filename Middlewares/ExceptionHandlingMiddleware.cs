using System.Net;

namespace Reddit.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this._requestDelegate = requestDelegate;
            this._logger = logger;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try{
                await _requestDelegate(context);

            }
            catch (Exception ex){
                _logger.LogError(ex, "Exception is thrown");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(" Unexpected error occured on the server.");
            }
        }

    }
}