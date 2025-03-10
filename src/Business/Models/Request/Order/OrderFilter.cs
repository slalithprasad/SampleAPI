using System.ComponentModel.DataAnnotations;
using Business.Validators;

namespace Business.Models.Request.Order;

public class OrderFilter
{
    [RequiredIfOthersNotNullValidator(nameof(PageSize), ErrorMessage = "Page Number is required when Page Size is provided.")]
    [Range(1, int.MaxValue, ErrorMessage = "Page Number must be greater than zero.")]
    public int? PageNumber { get; init; }

    [RequiredIfOthersNotNullValidator(nameof(PageNumber), ErrorMessage = "Page Size is required when Page Number is provided.")]
    [Range(1, int.MaxValue, ErrorMessage = "Page Size must be greater than zero.")]
    public int? PageSize { get; init; }

    [DateFormatValidator]
    public string? FromDate { get; init; }

    [DateFormatValidator]
    public string? ToDate { get; init; }

    public string? Search { get; init; }
}
