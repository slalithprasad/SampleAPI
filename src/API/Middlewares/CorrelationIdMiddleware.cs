namespace API.Middlewares;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        SetCorrelationId(context);

        await _next(context).ConfigureAwait(false);
    }

    private static void SetCorrelationId(HttpContext context)
    {
        string CorrelationIdHeader = "x-correlation-id";
        string correlationId = Guid.NewGuid().ToString();
        context.Request.Headers[CorrelationIdHeader] = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;
    }
}