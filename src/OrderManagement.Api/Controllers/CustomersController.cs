using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Mappings;
using OrderManagement.Api.Models.Customers;
using OrderManagement.Application.Common;
using OrderManagement.Application.Customers.Commands.CreateCustomer;
using OrderManagement.Application.Customers.Queries.GetCustomerById;
using OrderManagement.Application.Customers.Queries.GetCustomers;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ISender _sender;

    public CustomersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCustomerCommand(request.Name, request.Email);

        var result = await _sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ResultErrorType.NotFound => NotFound(result.Error),
                ResultErrorType.Conflict => Conflict(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        return Created($"/api/customers/{result.Value!.Id}", result.Value.ToResponse());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCustomerByIdQuery(id);

        var result = await _sender.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ResultErrorType.NotFound => NotFound(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        return Ok(result.Value!.ToResponse());
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
    CancellationToken cancellationToken)
    {
        var query = new GetCustomersQuery();

        var result = await _sender.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var customers = result.Value!.Select(customer => customer.ToResponse()).ToList();

        return Ok(customers);
    }
}