using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Mappings;
using OrderManagement.Api.Models.Orders;
using OrderManagement.Application.Common;
using OrderManagement.Application.Orders.Commands.CreateOrder;

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
                    ResultErrorType.Validation => BadRequest(result.Error),
                    ResultErrorType.NotFound => NotFound(result.Error),
                    ResultErrorType.Conflict => Conflict(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError)
                };
            }

            return Created($"/api/orders/{result.Value!.Id}", result.Value.ToResponse());
        }
    }
}
