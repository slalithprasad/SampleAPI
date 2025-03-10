using System;

namespace Domain.Models;

public class BaseEntity
{
    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public string? CreateBy { get; set; } = "System";
    public DateTime? ModifiedAt { get; set; } = DateTime.Now;
    public string? ModifiedBy { get; set; } = "System";
}