using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using SoundaryaProj.EnergyConsumption.Models;

namespace EnergyConsumption.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<EnergyRecord> EnergyRecords { get; set; } = null!;

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
        }
    }
}
