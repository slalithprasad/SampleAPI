using System.Net.Mime;
using Business.Interfaces;
using Business.Models.Request.Order;
using Business.Models.Response.Common;
using Microsoft.AspNetCore.Mvc;
using Business.Mappers;
using Domain.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]

    public class OrderController : ControllerBase
    {
        private readonly IOrderManager _manager;

        public OrderController(IOrderManager manager)
        {
            _manager = manager;
        }

        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse>> Get(int id, CancellationToken cancellationToken)
        {
            Order order = await _manager.GetAsync(id, cancellationToken).ConfigureAwait(false);
            return new ApiResponse(Result: order);
        }

        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> Get([FromQuery] OrderFilter filter, CancellationToken cancellationToken)
        {
            var (orders, totalRecords) = await _manager.GetAsync(filter, cancellationToken).ConfigureAwait(false);
            return new ApiResponse(Result: new
            {
                data = orders,
                totalRecords
            });
        }

        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            Order order = await _manager.CreateAsync(request.ToEntity(), cancellationToken).ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new { id = order.Id }, new ApiResponse());
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [Consumes(MediaTypeNames.Application.Json)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateOrderRequest request, CancellationToken cancellationToken)
        {
            await _manager.UpdateAsync(request.ToEntity(), cancellationToken).ConfigureAwait(false);
            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _manager.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
            return NoContent();
        }
    }
}
