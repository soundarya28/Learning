using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundaryaProj.Models.Entities;

public class Recommendation
{
    [Key]
    public int RecommendationId { get; set; }

    [Required]
    public int UserId { get; set; }

    public int? ApplianceId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public decimal? EstimatedSavingKwh { get; set; }

    public int Priority { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public bool IsRead { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(ApplianceId))]
    public Appliance? Appliance { get; set; }
}
