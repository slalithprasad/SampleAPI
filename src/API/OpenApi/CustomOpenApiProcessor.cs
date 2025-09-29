using System.Linq;
using System.Reflection;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using NJsonSchema;

namespace API.OpenApi;

public class CustomOpenApiProcessor : IOperationProcessor
{
    private const string SecuritySchemeName = "JWT";

    public bool Process(OperationProcessorContext context)
    {
        var method = context.MethodInfo;

        // existing header/summary/description handling...
        var headerAttrs = method.GetCustomAttributes(typeof(OpenApiHeaderAttribute), inherit: true)
            .Cast<OpenApiHeaderAttribute>();

        foreach (var attr in headerAttrs)
        {
            context.OperationDescription.Operation.Parameters.Add(new NSwag.OpenApiParameter
            {
                Name = attr.Name,
                Kind = NSwag.OpenApiParameterKind.Header,
                IsRequired = attr.Required,
                Description = attr.Description,
                Schema = new JsonSchema { Type = JsonObjectType.String }
            });
        }

        var summaryAttr = method.GetCustomAttribute<EndpointSummaryAttribute>();
        if (summaryAttr != null)
            context.OperationDescription.Operation.Summary = summaryAttr.Summary;

        var descAttr = method.GetCustomAttribute<EndpointDescriptionAttribute>();
        if (descAttr != null)
            context.OperationDescription.Operation.Description = descAttr.Description;

        // ----- security handling -----
        var hasAllowAnonymous =
            method.IsDefined(typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute), inherit: true) ||
            method.DeclaringType?.IsDefined(typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute), inherit: true) == true;

        if (!hasAllowAnonymous)
        {
            var hasAuthorize =
                method.IsDefined(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), inherit: true) ||
                method.DeclaringType?.IsDefined(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), inherit: true) == true;

            if (hasAuthorize)
            {
                if (context.OperationDescription.Operation.Security == null)
                    context.OperationDescription.Operation.Security = new System.Collections.Generic.List<NSwag.OpenApiSecurityRequirement>();

                var already = context.OperationDescription.Operation.Security
                    .Any(req => req.Keys.Contains(SecuritySchemeName));

                if (!already)
                {
                    context.OperationDescription.Operation.Security.Add(new NSwag.OpenApiSecurityRequirement
                    {
                        { SecuritySchemeName, new string[] { } }
                    });
                }
            }
        }

        return true;
    }
}
