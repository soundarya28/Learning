using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundaryaProj.Models.Entities;

public class Appliance
{
    [Key]
    public int ApplianceId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Category { get; set; }

    public int RatedPowerWatts { get; set; }

    public int Quantity { get; set; } = 1;

    public DateTimeOffset CreatedAt { get; set; }

    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}
