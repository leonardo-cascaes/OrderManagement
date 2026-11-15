namespace OrderManagement.Api.Models.Products;

public record ProductResponse(Guid Id, string Name, decimal Price, int Stock, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt);