using System;

namespace Business.Exceptions;

public static class ApiExceptions
{
    public static ApiException AE401 => new ApiException("AE401", "Unauthorized");
    public static ApiException AE403 => new ApiException("AE403", "Forbidden");
    public static ApiException AE404(string entity) => new ApiException("AE404", string.Format("{0} not found.", entity));
}
