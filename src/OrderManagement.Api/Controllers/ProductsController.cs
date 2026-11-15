using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Mappings;
using OrderManagement.Api.Models.Products;
using OrderManagement.Application.Common;
using OrderManagement.Application.Products.Commands.CreateProduct;
using OrderManagement.Application.Products.Queries.GetProducts;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(request.Name, request.Price, request.Stock);

        var result = await _sender.Send(command, cancellationToken);

        var product = result.Value!;

        return Created($"/api/products/{product.Id}", product.ToResponse());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);

        var result = await _sender.Send(query, cancellationToken);

        if (result.ErrorType == ResultErrorType.NotFound)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value!.ToResponse());
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetProductsQuery();

        var result = await _sender.Send(query, cancellationToken);

        var products = result.Value!
            .Select(product => product.ToResponse())
            .ToList();

        return Ok(products);
    }
}