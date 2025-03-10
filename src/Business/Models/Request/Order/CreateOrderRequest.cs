using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Business.Validators;

namespace Business.Models.Request.Order;

public class CreateOrderRequest
{
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
    [ValidOptionsValidator("draft", "active", ErrorMessage = "Status must be either 'active' or 'draft'")]
    [JsonPropertyName("status")]
    public string? Status { get; init; }
}
