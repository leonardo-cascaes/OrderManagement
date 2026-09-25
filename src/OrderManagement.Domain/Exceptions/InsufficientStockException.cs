namespace OrderManagement.Domain.Exceptions
{
    public class InsufficientStockException : InvalidOperationException
    {
        public InsufficientStockException() : base("Insufficient stock.") { }
    }
}
