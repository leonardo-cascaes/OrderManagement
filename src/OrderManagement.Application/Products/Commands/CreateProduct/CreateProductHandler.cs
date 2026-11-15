using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Persistence;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Products.Commands.CreateProduct;

public class CreateProductHandler
    : IRequestHandler<CreateProductCommand, Result<Product>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Product>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = new Product(command.Name, command.Price, command.Stock);

        _productRepository.Add(product);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Product>.Success(product);
    }
}