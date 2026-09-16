using OrderManagement.Domain.Common;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Entities
{
    public class Order : Entity
    {
        public Guid CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }
        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;
        public decimal Total => _orderItems.Sum(item => item.Quantity * item.UnitPrice);

        public Order(Guid customerId)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("CustomerId cannot be empty.", nameof(customerId));

            CustomerId = customerId;
            Status = OrderStatus.Pending;
        }

        public void AddItem(Guid productId, int quantity, decimal unitPrice)
        {
            _orderItems.Add(new OrderItem(productId, quantity, unitPrice));
        }

        public void Confirm()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be confirmed.");

            Status = OrderStatus.Confirmed;
        }

        public void Cancel()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be cancelled.");

            Status = OrderStatus.Cancelled;
        }

        public void Complete()
        {
            if (Status != OrderStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed orders can be completed.");

            Status = OrderStatus.Completed;
        }
    }
}
