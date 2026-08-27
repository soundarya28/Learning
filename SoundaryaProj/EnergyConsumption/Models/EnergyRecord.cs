using System;
using System.ComponentModel.DataAnnotations;

namespace SoundaryaProj.EnergyConsumption.Models;

public class EnergyRecord
{
    [Key]
    public int Id { get; set; }

    // Id of the appliance this reading belongs to
    public int ApplianceId { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    // Consumption in kWh
    public decimal ConsumptionKwh { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;
}
