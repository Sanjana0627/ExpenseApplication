using ExpenseApplication.Core.Entities;
using ExpenseApplication.Infrastructure.Data;
using System.Net;
using System.Text.Json;

namespace ExpenseApplication.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        // constructor
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // catches any unhandled exception from the rest of the pipeline, logs it
        // (to the database, falling back to the file/console logger if that fails),
        // and returns a generic error response instead of leaking the stack trace
        public async Task InvokeAsync(HttpContext context, ExpenseDbContext dbContext)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                try
                {
                    dbContext.ErrorLogs.Add(new ErrorLog
                    {
                        Source = $"{context.Request.Method} {context.Request.Path}",
                        Message = ex.Message,
                        StackTrace = ex.StackTrace,
                        CreatedDate = DateTime.UtcNow
                    });
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception logEx)
                {
                    _logger.LogError(ex, "Unhandled exception, and failed to write ErrorLog: {LogError}", logEx.Message);
                }
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "An unexpected error occurred. Please try again." }));
            }
        }
    }
}
