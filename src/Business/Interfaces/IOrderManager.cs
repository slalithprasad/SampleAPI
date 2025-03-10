using Business.Models.Request.Order;
using Domain.Models;

namespace Business.Interfaces;

public interface IOrderManager
{
    Task<Order> GetAsync(int id, CancellationToken cancellationToken);
    Task<(IEnumerable<Order>, long?)> GetAsync(OrderFilter filter, CancellationToken cancellationToken);
    Task<Order> CreateAsync(Order entity, CancellationToken cancellationToken);
    Task UpdateAsync(Order entity, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
