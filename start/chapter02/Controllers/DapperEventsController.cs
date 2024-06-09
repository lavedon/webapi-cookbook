using Microsoft.AspNetCore.Mvc;
using events.Services;
using events.Models;
using System.Text.Json;

namespace events.Controllers
{
    [Route("api/dapper/[controller]")]
    [ApiController]
    public class DapperEventsController : ControllerBase
    {
        private readonly IDapperService _service;
        private readonly ILogger<DapperEventsController> _logger;

        public DapperEventsController(IDapperService service, ILogger<DapperEventsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [EndpointSummary("Paged Event Registrations")]
        [EndpointDescription("This returns all the event registrations from our SQLite database, using Dapper")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyCollection<EventRegistrationDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "pageSize", "lastId" })] 
        public async Task<IActionResult> GetEventRegistrations([FromQuery] int pageSize = 10, [FromQuery] int lastId = 0)
        {
            try
            {
                _logger.LogInformation("Fetching event registrations with pageSize: {PageSize}, lastId: {LastId}", pageSize, lastId);
                var pagedResult = await _service.GetEventRegistrationsAsync(pageSize, lastId, Url);

                var paginationMetadata = new
                {
                    pagedResult.PageSize,
                    pagedResult.HasPreviousPage,
                    pagedResult.HasNextPage,
                    pagedResult.PreviousPageUrl,
                    pagedResult.NextPageUrl
                };

                var options = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata, options));

                return Ok(pagedResult.Items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching event registrations.");
                return StatusCode(500, "An error occurred while fetching event registrations.");
            }
        }
    }
}
