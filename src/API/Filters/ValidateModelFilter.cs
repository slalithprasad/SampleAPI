using Business.Models.Response.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

public class ValidateModelFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var firstErrorMessage = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value?.Errors?.Select(error => error.ErrorMessage) ?? Enumerable.Empty<string>())
                .FirstOrDefault();

            var result = new BadRequestObjectResult(new ApiResponse
            {
                IsSuccess = false,
                Error = firstErrorMessage
            });

            context.Result = result;
        }
    }
}