using OrderManagement.Api.Models.Customers;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Api.Mappings;

public static class CustomerMappingExtensions
{
    public static CustomerResponse ToResponse(this Customer customer)
    {
        return new CustomerResponse(customer.Id, customer.Name, customer.Email);
    }
}