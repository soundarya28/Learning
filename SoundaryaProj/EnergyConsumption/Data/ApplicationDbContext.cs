using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using SoundaryaProj.Models.Entities;

namespace SoundaryaProj.EnergyConsumption.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
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

        modelBuilder.Entity<Appliance>(entity =>
        {
            entity.ToTable("Appliances");
            entity.HasKey(a => a.ApplianceId);
            entity.Property(a => a.ApplianceId).ValueGeneratedOnAdd();
            entity.Property(a => a.Name).IsRequired().HasMaxLength(200);
            entity.Property(a => a.Category).HasMaxLength(100);
            entity.Property(a => a.IsActive).HasDefaultValue(true);

            entity.HasOne(a => a.User)
                  .WithMany()
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Consumption>(entity =>
        {
            entity.ToTable("Consumptions");
            entity.HasKey(c => c.ConsumptionId);
            entity.Property(c => c.ConsumptionId).ValueGeneratedOnAdd();
            entity.Property(c => c.EnergyKwh).HasColumnType("decimal(18,4)");

            entity.HasOne(c => c.User)
                  .WithMany()
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Appliance)
                  .WithMany()
                  .HasForeignKey(c => c.ApplianceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Prediction>(entity =>
        {
            entity.ToTable("Predictions");
            entity.HasKey(p => p.PredictionId);
            entity.Property(p => p.PredictionId).ValueGeneratedOnAdd();
            entity.Property(p => p.PredictedEnergyKwh).HasColumnType("decimal(18,4)");

            entity.HasOne(p => p.User)
                  .WithMany()
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Recommendation>(entity =>
        {
            entity.ToTable("Recommendations");
            entity.HasKey(r => r.RecommendationId);
            entity.Property(r => r.RecommendationId).ValueGeneratedOnAdd();
            entity.Property(r => r.Title).IsRequired().HasMaxLength(200);
            entity.Property(r => r.Description).HasMaxLength(2000);
            entity.Property(r => r.EstimatedSavingKwh).HasColumnType("decimal(18,4)");

            entity.HasOne(r => r.User)
                  .WithMany()
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Appliance)
                  .WithMany()
                  .HasForeignKey(r => r.ApplianceId)
                  .OnDelete(DeleteBehavior.SetNull);
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
            else if (entry.Entity is Appliance a && entry.State == EntityState.Added)
            {
                a.CreatedAt = now;
            }
            else if (entry.Entity is Consumption c && entry.State == EntityState.Added)
            {
                c.CreatedAt = now;
            }
            else if (entry.Entity is Prediction p && entry.State == EntityState.Added)
            {
                p.CreatedAt = now;
            }
            else if (entry.Entity is Recommendation r && entry.State == EntityState.Added)
            {
                r.CreatedAt = now;
            }
        }
    }
}
