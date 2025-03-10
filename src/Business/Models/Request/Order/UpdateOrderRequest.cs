using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Business.Validators;

namespace Business.Models.Request.Order;

public class UpdateOrderRequest
{
    [Required]
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [Required]
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    [JsonPropertyName("quantity")]
    public int Quantity { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    [JsonPropertyName("price")]
    public decimal Price { get; init; }

    [Required]
    [ValidOptionsValidator("draft", "active", "inactive", "delete", ErrorMessage = "Status must be either 'active', 'inactive', 'draft' or 'delete'")]
    [JsonPropertyName("status")]
    public string? Status { get; init; }
}
