namespace Business.Models.Response.Common;

public record ApiResponse(bool IsSuccess = true, object? Result = null, object? Error = null);