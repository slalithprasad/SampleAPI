using System;
using Business.Models.Request.Order;
using Domain.Enum;
using Domain.Models;

namespace Business.Mappers;

public static class OrderMapper
{
    public static Order ToEntity(this CreateOrderRequest request) => new Order
    {
        Name = request.Name,
        Quantity = request.Quantity,
        Price = request.Price,
        Status = Enum.TryParse<OrderStatus>(request.Status, true, out OrderStatus status) ? status.ToString() : null,
    };

    public static Order ToEntity(this UpdateOrderRequest request) => new Order
    {
        Id = request.Id,
        Name = request.Name,
        Quantity = request.Quantity,
        Price = request.Price,
        Status = Enum.TryParse<OrderStatus>(request.Status, true, out OrderStatus status) ? status.ToString() : null,
    };
}
