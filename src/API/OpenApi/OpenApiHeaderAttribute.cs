using System;

namespace API.OpenApi;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class OpenApiHeaderAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }
    public bool Required { get; }

    public OpenApiHeaderAttribute(string name, string description = "", bool required = false)
    {
        Name = name;
        Description = description;
        Required = required;
    }
}
