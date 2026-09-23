using OrderManagement.Domain.Enums;

namespace OrderManagement.Api.Models.Orders
{
    public record OrderResponse(
        Guid Id,
        Guid CustomerId,
        OrderStatus Status,
        decimal Total,
        IReadOnlyCollection<OrderItemResponse> OrderItems
    );

    public record OrderItemResponse(
        Guid Id,
        Guid ProductId,
        int Quantity,
        decimal UnitPrice
    );
}
