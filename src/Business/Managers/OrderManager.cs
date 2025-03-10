using System.Globalization;
using Business.Exceptions;
using Business.Interfaces;
using Business.Models.Request.Order;
using Domain.Contexts;
using Domain.Enum;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Managers;

public class OrderManager : IOrderManager
{
    private readonly AppDbContext _context;

    public OrderManager(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order> GetAsync(int id, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id && o.Status != OrderStatus.Delete.ToString(), cancellationToken).ConfigureAwait(false);

        if (order is null)
        {
            throw ApiExceptions.AE404("Order");
        }

        return order;
    }

    public async Task<(IEnumerable<Order>, long)> GetAsync(OrderFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<Order> query = _context.Orders.AsNoTracking();

        if (!string.IsNullOrEmpty(filter.Search))
        {
            query = query.Where(o => o.Name != null && o.Name.Contains(filter.Search));
        }

        if (!string.IsNullOrEmpty(filter.Status))
        {
            string? orderStatus = Enum.TryParse(filter.Status, true, out OrderStatus status) ? status.ToString() : null;

            if (!string.IsNullOrEmpty(orderStatus))
            {
                query = query.Where(o => o.Status == orderStatus);
            }
        }

        DateTime? fromDate = string.IsNullOrEmpty(filter.FromDate) ? null :
            DateTime.TryParseExact(filter.FromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime from) ? from : null;

        DateTime? toDate = string.IsNullOrEmpty(filter.ToDate) ? null :
            DateTime.TryParseExact(filter.ToDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime to) ? to : null;

        if (fromDate.HasValue && !toDate.HasValue)
        {
            var startOfDay = fromDate.Value.Date;
            var endOfDay = fromDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(po => po.CreatedAt >= startOfDay && po.CreatedAt <= endOfDay);
        }
        else if (fromDate.HasValue && toDate.HasValue)
        {
            var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(po => po.CreatedAt >= fromDate.Value && po.CreatedAt <= endOfDay);
        }

        query = query.Where(x => x.Status != OrderStatus.Delete.ToString());

        long totalRecords = await query.LongCountAsync(cancellationToken).ConfigureAwait(false);

        if (filter.PageNumber.HasValue && filter.PageSize.HasValue)
        {
            int skip = (filter.PageNumber.Value - 1) * filter.PageSize.Value;
            query = query.Skip(skip).Take(filter.PageSize.Value);
        }

        var orders = await query.ToListAsync(cancellationToken).ConfigureAwait(false);

        return (orders, totalRecords);
    }



    public async Task<Order> CreateAsync(Order entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.Orders.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return entity;
    }

    public async Task UpdateAsync(Order entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Order existingOrder = await GetAsync(entity.Id, cancellationToken).ConfigureAwait(false);

        if (existingOrder != null)
        {
            existingOrder.Name = entity.Name;
            existingOrder.Quantity = entity.Quantity;
            existingOrder.Price = entity.Price;
            existingOrder.Status = entity.Status;
            existingOrder.ModifiedAt = DateTime.UtcNow;
        }

        _context.Orders.Update(entity);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        Order order = await GetAsync(id, cancellationToken).ConfigureAwait(false);
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
