namespace OrderManagement.Api.Models.Orders
{
    public record CreateOrderRequest(
        Guid CustomerId,
        Guid ProductId,
        int Quantity
    );
}
