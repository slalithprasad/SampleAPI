using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Business.Validators;

namespace Business.Models.Request.Order;

public class OrderFilter
{
    [RequiredIfOthersNotNullValidator(nameof(PageSize), ErrorMessage = "Page Number is required when Page Size is provided.")]
    [Range(1, int.MaxValue, ErrorMessage = "Page Number must be greater than zero.")]
    [JsonPropertyName("pageNumber")]
    public int? PageNumber { get; init; }

    [RequiredIfOthersNotNullValidator(nameof(PageNumber), ErrorMessage = "Page Size is required when Page Number is provided.")]
    [Range(1, int.MaxValue, ErrorMessage = "Page Size must be greater than zero.")]
    [JsonPropertyName("pageSize")]
    public int? PageSize { get; init; }

    [DateFormatValidator]
    [JsonPropertyName("fromDate")]
    public string? FromDate { get; init; }

    [DateFormatValidator]
    [JsonPropertyName("toDate")]
    public string? ToDate { get; init; }

    [JsonPropertyName("search")]
    public string? Search { get; init; }

    [JsonPropertyName("status")]
    [ValidOptionsValidator("draft", "active", "inactive", ErrorMessage = "Status must be either 'draft', 'active' or 'inactive'")]
    public string? Status { get; init; }
}