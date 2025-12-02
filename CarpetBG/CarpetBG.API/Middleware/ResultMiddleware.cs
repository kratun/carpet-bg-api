namespace CarpetBG.API.Middleware;

public class ResultMiddleware
{
    private readonly RequestDelegate _next;

    public ResultMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = "Internal Server Error", details = ex.Message });
        }
    }
}