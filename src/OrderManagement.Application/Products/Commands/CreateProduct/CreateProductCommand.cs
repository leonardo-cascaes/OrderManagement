using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(string Name, decimal Price, int Stock) : IRequest<Result<Product>>;