using System.ComponentModel;
using System.Net.Mime;
using Business.Interfaces;
using Business.Models.Request.Order;
using Business.Models.Response.Common;
using Microsoft.AspNetCore.Mvc;
using Business.Mappers;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{
    /// <summary>Manage customer orders.</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Tags("Orders")] // OpenAPI tag for the whole controller
    public class OrderController : ControllerBase
    {
        private readonly IOrderManager _manager;
        public OrderController(IOrderManager manager) => _manager = manager;

        [HttpGet("{id:int}")]
        [EndpointName("Orders_GetById")]
        [EndpointSummary("Get order by ID")]
        [EndpointDescription("Returns a single order by its numeric identifier.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Get([FromRoute, Description("Numeric ID of the order to retrieve.")] int id, CancellationToken cancellationToken)
        {
            Order order = await _manager.GetAsync(id, cancellationToken).ConfigureAwait(false);
            return new ApiResponse(Result: order);
        }

        [HttpGet]
        [EndpointName("Orders_List")]
        [EndpointSummary("List orders")]
        [EndpointDescription("Retrieves orders with optional filtering, sorting, and pagination.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Get([FromQuery, Description("Filter criteria for searching orders.")] OrderFilter filter, CancellationToken cancellationToken)
        {
            var (orders, totalRecords) = await _manager.GetAsync(filter, cancellationToken).ConfigureAwait(false);
            return new ApiResponse(Result: new { data = orders, totalRecords });
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [EndpointName("Orders_Create")]
        [EndpointSummary("Create new order")]
        [EndpointDescription("Creates a new order with the provided details.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody, Description("Order details to create.")] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            Order order = await _manager.CreateAsync(request.ToEntity(), cancellationToken).ConfigureAwait(false);
            return CreatedAtAction(nameof(Get), new { id = order.Id }, new ApiResponse());
        }

        [HttpPut]
        [Consumes(MediaTypeNames.Application.Json)]
        [EndpointName("Orders_Update")]
        [EndpointSummary("Update order")]
        [EndpointDescription("Updates the details of an existing order.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody, Description("Updated order details.")] UpdateOrderRequest request, CancellationToken cancellationToken)
        {
            await _manager.UpdateAsync(request.ToEntity(), cancellationToken).ConfigureAwait(false);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [EndpointName("Orders_Delete")]
        [EndpointSummary("Delete order")]
        [EndpointDescription("Deletes a specific order by its ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute, Description("Numeric ID of the order to delete.")] int id, CancellationToken cancellationToken)
        {
            await _manager.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
            return NoContent();
        }
    }
}
