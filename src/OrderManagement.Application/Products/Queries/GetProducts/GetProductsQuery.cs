using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Products.Queries.GetProducts;

public record GetProductsQuery() : IRequest<Result<IReadOnlyCollection<Product>>>;