using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundaryaProj.Models.Entities;

public class Consumption
{
    [Key]
    public int ConsumptionId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ApplianceId { get; set; }

    public DateTimeOffset ConsumptionDate { get; set; }

    public double HoursUsed { get; set; }

    public decimal EnergyKwh { get; set; }

    [MaxLength(100)]
    public string? Source { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(ApplianceId))]
    public Appliance? Appliance { get; set; }
}
