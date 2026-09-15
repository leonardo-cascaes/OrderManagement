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
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity cannot be zero or negative value.", nameof(quantity));
            }

            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

    }
}
