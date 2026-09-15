using OrderManagement.Domain.Common;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Entities
{
    public class Order : Entity
    {
        public Guid CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }

        public Order(Guid customerId)
        {
            CustomerId = customerId;
            Status = OrderStatus.Pending;
        }
    }
}
