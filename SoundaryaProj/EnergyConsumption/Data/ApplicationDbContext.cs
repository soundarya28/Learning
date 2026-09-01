using Microsoft.EntityFrameworkCore;
using Models.Entities;
using SoundaryaProj.EnergyConsumption.Models;
using SoundaryaProj.Models.Entities;

namespace EnergyConsumption.Data;

public class ApplicationDbContext : DbContext
{
    // local user model (used by AuthService) and energy records
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<EnergyRecord> EnergyRecords { get; set; } = null!;

    // shared/domain models from the Models project
    public DbSet<Appliance> Appliances { get; set; } = null!;
    public DbSet<Consumption> Consumptions { get; set; } = null!;
    public DbSet<Prediction> Predictions { get; set; } = null!;
    public DbSet<Recommendation> Recommendations { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(u => u.UserId);
            entity.Property(u => u.UserId).ValueGeneratedOnAdd();

            entity.Property(u => u.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(256);

            entity.HasIndex(u => u.Email)
                  .IsUnique();

            entity.Property(u => u.PasswordHash)
                  .IsRequired()
                  .HasMaxLength(512);

            // Keep CreatedAt/UpdatedAt managed in SaveChanges to avoid provider-specific SQL.
            entity.Property(u => u.IsActive)
                  .HasDefaultValue(true);
        });

        // Appliances (from Models project)
        modelBuilder.Entity<Appliance>(entity =>
        {
            entity.ToTable("Appliances");
            entity.HasKey(a => a.ApplianceId);
            entity.Property(a => a.ApplianceId).ValueGeneratedOnAdd();

            entity.Property(a => a.Name).IsRequired().HasMaxLength(200);
            entity.Property(a => a.Category).HasMaxLength(100);
            entity.Property(a => a.RatedPowerWatts).IsRequired();
            entity.Property(a => a.Quantity).HasDefaultValue(1);
            entity.Property(a => a.IsActive).HasDefaultValue(true);

            entity.HasIndex(a => a.UserId);
        });

        // Consumptions (from Models project)
        modelBuilder.Entity<Consumption>(entity =>
        {
            entity.ToTable("Consumptions");
            entity.HasKey(c => c.ConsumptionId);
            entity.Property(c => c.ConsumptionId).ValueGeneratedOnAdd();

            entity.Property(c => c.UserId).IsRequired();
            entity.Property(c => c.ApplianceId).IsRequired();
            entity.Property(c => c.ConsumptionDate).IsRequired();
            entity.Property(c => c.EnergyKwh).IsRequired();

            entity.HasIndex(c => c.UserId);
            entity.HasIndex(c => c.ApplianceId);
        });

        // Predictions
        modelBuilder.Entity<Prediction>(entity =>
        {
            entity.ToTable("Predictions");
            entity.HasKey(p => p.PredictionId);
            entity.Property(p => p.PredictionId).ValueGeneratedOnAdd();

            entity.Property(p => p.UserId).IsRequired();
            entity.Property(p => p.PredictionDate).IsRequired();
            entity.Property(p => p.TargetDate).IsRequired();
            entity.Property(p => p.PredictedEnergyKwh).IsRequired();
        });

        // Recommendations
        modelBuilder.Entity<Recommendation>(entity =>
        {
            entity.ToTable("Recommendations");
            entity.HasKey(r => r.RecommendationId);
            entity.Property(r => r.RecommendationId).ValueGeneratedOnAdd();

            entity.Property(r => r.UserId).IsRequired();
            entity.Property(r => r.Title).IsRequired().HasMaxLength(200);
            entity.Property(r => r.Description).HasMaxLength(2000);
            entity.Property(r => r.IsRead).HasDefaultValue(false);
            entity.Property(r => r.IsRead).HasDefaultValue(false);
        });

        modelBuilder.Entity<EnergyRecord>(entity =>
        {
            entity.ToTable("EnergyRecords");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.ApplianceId)
                  .IsRequired();

            entity.Property(e => e.Timestamp)
                  .IsRequired();

            entity.Property(e => e.ConsumptionKwh)
                  .IsRequired();

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);

            // Optionally add indexes
            entity.HasIndex(e => e.ApplianceId);
            entity.HasIndex(e => e.Timestamp);
        });

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        ApplyAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditFields()
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is User u)
            {
                if (entry.State == EntityState.Added)
                {
                    u.CreatedAt = now;
                    u.UpdatedAt = now;
                    u.IsActive = u.IsActive;
                }
                else if (entry.State == EntityState.Modified)
                {
                    u.UpdatedAt = now;
                }
            }
            else if (entry.Entity is EnergyRecord er)
            {
                if (entry.State == EntityState.Added)
                {
                    er.CreatedAt = now;
                    er.UpdatedAt = now;
                    er.IsActive = er.IsActive;
                }
                else if (entry.State == EntityState.Modified)
                {
                    er.UpdatedAt = now;
                }
            }
            else if (entry.Entity is Appliance app)
            {
                if (entry.State == EntityState.Added)
                {
                    app.CreatedAt = now;
                    app.IsActive = app.IsActive;
                }
            }
            else if (entry.Entity is Consumption cons)
            {
                if (entry.State == EntityState.Added)
                {
                    cons.CreatedAt = now;
                }
            }
            else if (entry.Entity is Prediction pred)
            {
                if (entry.State == EntityState.Added)
                {
                    pred.CreatedAt = now;
                }
            }
            else if (entry.Entity is Recommendation rec)
            {
                if (entry.State == EntityState.Added)
                {
                    rec.CreatedAt = now;
                    rec.IsRead = rec.IsRead;
                }
            }
        }
    }
}
