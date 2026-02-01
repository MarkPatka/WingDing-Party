using Microsoft.AspNetCore.Mvc;
using EventService.Application.Services;
using EventService.Contracts.Events;

namespace EventService.Api.Controllers;

public class ConfigurationsController : ControllerBase
{
    private readonly IConfigurationService _configurationService;

    public ConfigurationsController(IConfigurationService configurationService) =>
        _configurationService = configurationService;

    [HttpGet("Show")]
    public async Task<IActionResult> ShowConfiguration()
    {
        var dbConfig = _configurationService.GetDatabaseInfo();
        var pgAdminConfig = _configurationService.GetPgAdminSettings();
        var apiConfig = _configurationService.GetApiSettings();

        return Ok();
    }
}
