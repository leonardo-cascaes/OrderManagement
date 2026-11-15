using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Products.Queries.GetProducts;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, Result<IReadOnlyCollection<Product>>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<IReadOnlyCollection<Product>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);

        return Result<IReadOnlyCollection<Product>>.Success(products);
    }
}