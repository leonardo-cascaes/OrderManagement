using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

public class GetProductByIdHandler
    : IRequestHandler<GetProductByIdQuery, Result<Product>>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<Product>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            return Result<Product>.Failure("Product not found.", ResultErrorType.NotFound);
        }

        return Result<Product>.Success(product);
    }
}