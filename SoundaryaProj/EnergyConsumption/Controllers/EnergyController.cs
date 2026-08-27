using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SoundaryaProj.EnergyConsumption.Services;
using SoundaryaProj.EnergyConsumption.Models;

namespace SoundaryaProj.EnergyConsumption.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnergyController : ControllerBase
{
    private readonly IEnergyService _service;
    private readonly ILogger<EnergyController> _logger;

    public EnergyController(IEnergyService service, ILogger<EnergyController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEnergyDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await _service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? applianceId, [FromQuery] DateTimeOffset? from = null, [FromQuery] DateTimeOffset? to = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var result = await _service.GetAllAsync(applianceId, page, pageSize, from, to, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateEnergyDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _service.UpdateAsync(id, dto, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        try
        {
            await _service.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("appliance/{applianceId:int}")]
    public async Task<IActionResult> GetByAppliance([FromRoute] int applianceId, [FromQuery] DateTimeOffset? from = null, [FromQuery] DateTimeOffset? to = null, CancellationToken cancellationToken = default)
    {
        var items = await _service.GetByApplianceAsync(applianceId, from, to, cancellationToken);
        return Ok(items);
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportCsv(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "File is required." });

        using var stream = file.OpenReadStream();
        var created = await _service.ImportCsvAsync(stream, cancellationToken);
        if (created == 0) return BadRequest(new { message = "No valid records found." });
        return Ok(new { created });
    }
}
