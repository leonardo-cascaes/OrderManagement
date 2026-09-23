using OrderManagement.Api.Models.Orders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Api.Mappings
{
    public static class OrderMappingExtensions
    {
        public static OrderResponse ToResponse(this Order order)
        {
            return new OrderResponse(
                order.Id,
                order.CustomerId,
                order.Status,
                order.Total,
                order.OrderItems
                    .Select(item => new OrderItemResponse(
                        item.Id,
                        item.ProductId,
                        item.Quantity,
                        item.UnitPrice))
                    .ToList());
        }
    }
}
