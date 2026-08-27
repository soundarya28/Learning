using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoundaryaProj.EnergyConsumption.Services;

public sealed record CreateEnergyDto
{
    [Required]
    public int ApplianceId { get; init; }

    [Required]
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// Consumption in kWh. If not provided, supply PowerWatts and Hours to calculate.
    /// </summary>
    public decimal ConsumptionKwh { get; init; }

    /// <summary>
    /// Optional: Power in watts used to calculate kWh when ConsumptionKwh is not provided.
    /// </summary>
    public decimal? PowerWatts { get; init; }

    /// <summary>
    /// Optional: Hours of usage used to calculate kWh when ConsumptionKwh is not provided.
    /// </summary>
    public decimal? Hours { get; init; }
}

public sealed record UpdateEnergyDto
{
    [Required]
    public int ApplianceId { get; init; }

    [Required]
    public DateTimeOffset Timestamp { get; init; }

    public decimal ConsumptionKwh { get; init; }

    public decimal? PowerWatts { get; init; }

    public decimal? Hours { get; init; }
}

public sealed record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);

public sealed record ImportResult(int CreatedCount);
