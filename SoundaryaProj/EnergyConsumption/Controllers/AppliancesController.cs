using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using SoundaryaProj.EnergyConsumption.Data;

namespace EnergyConsumption.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppliancesController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAppliances()
    {
        var appliances = await context.Appliances
            .Where(a => a.IsActive)
            .ToListAsync();

        return Ok(appliances);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppliance(int id)
    {
        var appliance = await context.Appliances
            .FirstOrDefaultAsync(a => a.ApplianceId == id && a.IsActive);

        if (appliance == null)
            return NotFound();

        return Ok(appliance);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppliance(Appliance appliance)
    {
        appliance.CreatedAt = DateTime.UtcNow;
        appliance.IsActive = true;

        context.Appliances.Add(appliance);
        await context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAppliance),
            new { id = appliance.ApplianceId },
            appliance);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppliance(
    int id,
    Appliance updatedAppliance)
    {
        var appliance = await context.Appliances
            .FirstOrDefaultAsync(a =>
                a.ApplianceId == id &&
                a.IsActive);

        if (appliance == null)
            return NotFound();

        appliance.Name = updatedAppliance.Name;
        appliance.Category = updatedAppliance.Category;
        appliance.RatedPowerWatts = updatedAppliance.RatedPowerWatts;
        appliance.Quantity = updatedAppliance.Quantity;

        await context.SaveChangesAsync();

        return Ok(appliance);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppliance(int id)
    {
        var appliance = await context.Appliances
            .FirstOrDefaultAsync(a =>
                a.ApplianceId == id &&
                a.IsActive);

        if (appliance == null)
            return NotFound();

        appliance.IsActive = false;

        await context.SaveChangesAsync();

        return NoContent();
    }

}
