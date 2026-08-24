using Models.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundaryaProj.Models.Entities;

public class Prediction
{
    [Key]
    public int PredictionId { get; set; }

    [Required]
    public int UserId { get; set; }

    public DateTimeOffset PredictionDate { get; set; }

    public DateTimeOffset TargetDate { get; set; }

    public decimal PredictedEnergyKwh { get; set; }

    [MaxLength(100)]
    public string? PredictionType { get; set; }

    [MaxLength(50)]
    public string? ModelVersion { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}
