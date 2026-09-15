using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Entities
{
    public class OrderItem : Entity
    {
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public OrderItem(Guid productId, int quantity, decimal unitPrice)
        {
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

    }
}
