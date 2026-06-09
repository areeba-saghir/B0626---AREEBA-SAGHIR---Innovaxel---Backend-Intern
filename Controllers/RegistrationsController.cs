using EventRegistrationsApi.Models;
using EventRegistrationsApi.Services;
using EventRegistrationsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventRegistrationApi.Controllers;

[ApiController]
[Route("api/registrations")]
public class RegistrationsController : ControllerBase
{
    private readonly RegistrationService _service;
    public RegistrationsController(RegistrationService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] CreateRegistrationRequest req)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault() ?? "Validation failed.";
            return BadRequest(new ErrorResponse { Error = errors });
        }

        var result = await _service.RegisterAsync(req);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new ErrorResponse { Error = result.Error! });

        return StatusCode(201, new
        {
            message = "Registration successful.",
            registration = result.Value
        });
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(string id)
    {
        var result = await _service.CancelAsync(id);
        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new ErrorResponse { Error = result.Error! });

        return Ok(new
        {
            message = "Registration cancelled successfully.",
            registration = result.Value
        });
    }


    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery(Name = "event_id")] string? eventId = null,
        [FromQuery] string? status = null)
    {
        var result = await _service.ListAsync(eventId, status);
        return Ok(result);
    }
}
