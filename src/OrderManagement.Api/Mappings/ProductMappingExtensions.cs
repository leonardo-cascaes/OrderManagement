using OrderManagement.Api.Models.Products;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Api.Mappings;

public static class ProductMappingExtensions
{
    public static ProductResponse ToResponse(this Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Price,
            product.Stock,
            product.CreatedAt,
            product.UpdatedAt
        );
    }
}