namespace SynthShop.Middleware;

public class ExampleMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next.Invoke(context);
    }
}