using API.Attributes;
using Business.Models.Response.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    /// <summary>
    /// Authentication related endpoints.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(ILogger<AuthenticationController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Check whether the current request is authenticated.
        /// </summary>
        /// <remarks>
        /// This endpoint can be used by the UI to verify the provided JWT / authentication token.
        /// </remarks>
        /// <param name="cancellationToken">Cancellation token provided by the framework.</param>
        /// <returns>200 OK when authenticated, 401 when not.</returns>
        [Authorize]
        [HttpGet]
        [Route("me")]
        [ApiVersion("1.0")]
        [EndpointName("Authenticate_User_V1")]
        [EndpointSummary("Check if user is authenticated")]
        [EndpointDescription("Returns 200 if the request has a valid authentication principal.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAsyncV1(CancellationToken cancellationToken)
        {
            return new ApiResponse();
        }


        [Authorize]
        [HttpGet]
        [Route("me")]
        [ApiVersion("2.0")]
        [EndpointName("Authenticate_User_V2")]
        [EndpointSummary("Check if user is authenticated")]
        [EndpointDescription("Returns 200 if the request has a valid authentication principal.")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAsyncV2(CancellationToken cancellationToken)
        {
            return new ApiResponse();
        }
    }
}
