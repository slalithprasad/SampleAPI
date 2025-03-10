using System.Net;
using System.Text.Json;
using Business.Exceptions;
using Business.Models.Response.Common;

namespace API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException oex)
        {
            await HandleOperationCancelledExceptionAsync(context, oex).ConfigureAwait(false);
        }
        catch (ApiException apiex)
        {
            await HandleApiExceptionAsync(context, apiex).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
    }
    
    private Task HandleOperationCancelledExceptionAsync(HttpContext context, OperationCanceledException exception)
    {
        var response = new ApiResponse(IsSuccess: false, Error: new
        {
            ErrorCode = (HttpStatusCode)499,
            ErrorMessage = "Operation Cancelled"
        });

        return WriteResponse(context, response, (HttpStatusCode)499);
    }

    private Task HandleApiExceptionAsync(HttpContext context, ApiException exception)
    {
        _logger.LogError(exception, "{message}", exception.Message);

        ApiResponse response = new ApiResponse(IsSuccess: false, Error: new
        {
            ErrorCode = exception.ErrorCode,
            ErrorMessage = exception.ErrorMessage
        });

        HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;

        switch (exception.ErrorCode)
        {
            case "AE401":
                httpStatusCode = HttpStatusCode.Unauthorized;
                break;
            case "AE403":
                httpStatusCode = HttpStatusCode.Forbidden;
                break;
            case "AE404":
                httpStatusCode = HttpStatusCode.NotFound;
                break;
            case "AE410":
                httpStatusCode = HttpStatusCode.Gone;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = Convert.ToInt32(httpStatusCode);
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "{message}", exception.Message);

        var response = new ApiResponse(IsSuccess: false, Error: new
        {
            ErrorCode = HttpStatusCode.InternalServerError,
            ErrorMessage = "Something went wrong, Please try again!"
        });

        return WriteResponse(context, response, HttpStatusCode.InternalServerError);
    }

    private static Task WriteResponse(HttpContext context, ApiResponse response, HttpStatusCode statusCode)
    {
        var jsonResponse = JsonSerializer.Serialize(response);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(jsonResponse);
    }
}
