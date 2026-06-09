using EventRegistrationsApi.Models;
using EventRegistrationsApi.Services;
using EventRegistrationsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventRegistrationsApi.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly EventService _service;
    public EventsController(EventService service) => _service = service;


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventRequest req)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault() ?? "Validation failed.";
            return BadRequest(new ErrorResponse { Error = errors });
        }

        var result = await _service.CreateAsync(req);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new ErrorResponse { Error = result.Error! });

        return StatusCode(201, new { message = "Event created successfully.", @event = result.Value });
    }


    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] bool upcoming = false,
        [FromQuery] string sort = "asc")
    {
        var result = await _service.ListAsync(upcoming, sort);
        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new ErrorResponse { Error = result.Error! });

        return Ok(result.Value);
    }
}
