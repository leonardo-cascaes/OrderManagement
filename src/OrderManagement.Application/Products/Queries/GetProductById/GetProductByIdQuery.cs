using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Domain.Entities;

public record GetProductByIdQuery(Guid Id) : IRequest<Result<Product>>;