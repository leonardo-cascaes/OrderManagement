namespace OrderManagement.Api.Models.Products;

public record CreateProductRequest(string Name, decimal Price, int Stock);