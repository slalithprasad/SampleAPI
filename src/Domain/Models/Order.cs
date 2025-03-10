namespace Domain.Models;

public class Order : BaseEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? Status { get; set; }
}
