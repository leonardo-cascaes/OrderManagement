using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.IntegrationTests.TestSupport.Stubs
{
    internal class OrderRepositoryStub : IOrderRepository
    {
        public void Add(Order order)
        {
        }
    }
}
