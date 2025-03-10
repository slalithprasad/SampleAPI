using System;
using System.ComponentModel.DataAnnotations;
using Business.Validators;

namespace Business.Models.Request.Order;

public class CreateOrderRequest
{
    [Required]
    public string? Name { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; init; }

    [Required]
    [ValidOptionsValidator("draft", "active", ErrorMessage = "Status must be either 'active' or 'draft'")]
    public string? Status { get; init; }
}
