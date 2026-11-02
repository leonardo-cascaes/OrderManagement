using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        void Add(Order order);
    }
}
