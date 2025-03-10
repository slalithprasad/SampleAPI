using System;
using System.ComponentModel.DataAnnotations;
using Business.Validators;

namespace Business.Models.Request.Order;

public class UpdateOrderRequest
{
    [Required]
    public int Id { get; init; }

    [Required]
    public string? Name { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; init; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; init; }

    [Required]
    [ValidOptionsValidator("draft", "active", "inactive", "delete", ErrorMessage = "Status must be either 'active', 'inactive', 'draft' or 'delete'")]
    public string? Status { get; init; }
}
