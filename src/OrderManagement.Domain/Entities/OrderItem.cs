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
                throw new ArgumentException("Quantity cannot be zero or negative value.", nameof(quantity));

            if (unitPrice <= 0m)
                throw new ArgumentException("UnitPrice must be greater than zero.", nameof(unitPrice));

            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

    }
}
