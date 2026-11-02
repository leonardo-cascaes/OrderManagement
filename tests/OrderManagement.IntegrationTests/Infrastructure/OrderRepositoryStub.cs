using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.IntegrationTests.Infrastructure
{
    internal class OrderRepositoryStub : IOrderRepository
    {
        public void Add(Order order)
        {
        }
    }
}
