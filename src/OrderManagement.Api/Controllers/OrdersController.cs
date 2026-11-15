using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Mappings;
using OrderManagement.Api.Models.Orders;
using OrderManagement.Application.Common;
using OrderManagement.Application.Orders.Commands.CancelOrder;
using OrderManagement.Application.Orders.Commands.CompleteOrder;
using OrderManagement.Application.Orders.Commands.ConfirmOrder;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Application.Orders.Queries.GetOrderById;
using OrderManagement.Application.Orders.Queries.GetOrders;

namespace OrderManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ISender _sender;

        public OrdersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateOrderCommand(request.CustomerId, request.ProductId, request.Quantity);

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

            return Created($"/api/orders/{result.Value!.Id}", result.Value.ToResponse());
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetOrderByIdQuery(id);

            var result = await _sender.Send(query, cancellationToken);

            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ResultErrorType.NotFound => NotFound(result.Error),
                    ResultErrorType.Conflict => Conflict(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError)
                };
            }

            return Ok(result.Value!.ToResponse());
        }

        [HttpPost("{id:guid}/confirm")]
        public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
        {
            var command = new ConfirmOrderCommand(id);

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

            return Ok(result.Value!.ToResponse());
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetOrdersQuery();

            var result = await _sender.Send(query, cancellationToken);

            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ResultErrorType.NotFound => NotFound(result.Error),
                    ResultErrorType.Conflict => Conflict(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError)
                };
            }

            var orders = result.Value!
                .Select(order => order.ToResponse())
                .ToList();

            return Ok(orders);
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
        {
            var command = new CancelOrderCommand(id);

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

            return Ok(result.Value!.ToResponse());
        }

        [HttpPost("{id:guid}/complete")]
        public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
        {
            var command = new CompleteOrderCommand(id);

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

            return Ok(result.Value!.ToResponse());
        }
    }
}
