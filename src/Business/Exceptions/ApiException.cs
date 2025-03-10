using System.Text.Json.Serialization;

namespace Business.Exceptions;

public class ApiException : ApplicationException
{
    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; init; }

    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; init; }

    public ApiException(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
