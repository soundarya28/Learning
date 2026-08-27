using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SoundaryaProj.EnergyConsumption.Data;
using SoundaryaProj.EnergyConsumption.Models;

namespace SoundaryaProj.EnergyConsumption.Services;

public class EnergyService : IEnergyService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<EnergyService> _logger;

    public EnergyService(ApplicationDbContext db, ILogger<EnergyService> logger)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<EnergyRecord> CreateAsync(CreateEnergyDto dto, CancellationToken cancellationToken)
    {
        // Determine consumption in kWh. Use provided ConsumptionKwh if > 0, otherwise compute from PowerWatts and Hours.
        decimal energyKwh;
        if (dto.ConsumptionKwh > 0m)
        {
            energyKwh = dto.ConsumptionKwh;
        }
        else if (dto.PowerWatts.HasValue && dto.Hours.HasValue)
        {
            // energy (kWh) = power (W) / 1000 * hours
            energyKwh = (dto.PowerWatts.Value / 1000m) * dto.Hours.Value;
        }
        else
        {
            throw new ArgumentException("Either ConsumptionKwh or both PowerWatts and Hours must be provided.");
        }

        var entity = new EnergyRecord
        {
            ApplianceId = dto.ApplianceId,
            Timestamp = dto.Timestamp,
            ConsumptionKwh = energyKwh,
            IsActive = true
        };

        _db.EnergyRecords.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<PagedResult<EnergyRecord>> GetAllAsync(int? applianceId, int page, int pageSize, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 1000);

        var query = _db.EnergyRecords.AsNoTracking().Where(e => e.IsActive);
        if (applianceId.HasValue)
            query = query.Where(e => e.ApplianceId == applianceId.Value);

        if (from.HasValue)
            query = query.Where(e => e.Timestamp >= from.Value);
        if (to.HasValue)
            query = query.Where(e => e.Timestamp <= to.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(e => e.Timestamp)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync(cancellationToken);

        return new PagedResult<EnergyRecord>(items, total, page, pageSize);
    }

    public async Task<EnergyRecord?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _db.EnergyRecords.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id && e.IsActive, cancellationToken);
    }

    public async Task UpdateAsync(int id, UpdateEnergyDto dto, CancellationToken cancellationToken)
    {
        var entity = await _db.EnergyRecords.SingleOrDefaultAsync(e => e.Id == id && e.IsActive, cancellationToken);
        if (entity == null) throw new KeyNotFoundException("Energy record not found.");

        // Recompute if needed
        decimal energyKwh;
        if (dto.ConsumptionKwh > 0m)
        {
            energyKwh = dto.ConsumptionKwh;
        }
        else if (dto.PowerWatts.HasValue && dto.Hours.HasValue)
        {
            energyKwh = (dto.PowerWatts.Value / 1000m) * dto.Hours.Value;
        }
        else
        {
            throw new ArgumentException("Either ConsumptionKwh or both PowerWatts and Hours must be provided.");
        }

        entity.Timestamp = dto.Timestamp;
        entity.ConsumptionKwh = energyKwh;
        entity.ApplianceId = dto.ApplianceId;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _db.EnergyRecords.SingleOrDefaultAsync(e => e.Id == id && e.IsActive, cancellationToken);
        if (entity == null) throw new KeyNotFoundException("Energy record not found.");

        entity.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<EnergyRecord>> GetByApplianceAsync(int applianceId, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken)
    {
        var query = _db.EnergyRecords.AsNoTracking().Where(e => e.IsActive && e.ApplianceId == applianceId);
        if (from.HasValue) query = query.Where(e => e.Timestamp >= from.Value);
        if (to.HasValue) query = query.Where(e => e.Timestamp <= to.Value);

        return await query.OrderByDescending(e => e.Timestamp).ToListAsync(cancellationToken);
    }

    public async Task<int> ImportCsvAsync(Stream csvStream, CancellationToken cancellationToken)
    {
        if (csvStream == null) throw new ArgumentNullException(nameof(csvStream));

        var created = new List<EnergyRecord>();

        using var reader = new StreamReader(csvStream);
        string? header = await reader.ReadLineAsync();
        if (header == null) return 0;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',');
            if (parts.Length < 3)
            {
                _logger.LogWarning("Skipping malformed CSV line: {Line}", line);
                continue;
            }

            if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var applianceId))
            {
                _logger.LogWarning("Invalid ApplianceId in line: {Line}", line);
                continue;
            }

            if (!DateTimeOffset.TryParse(parts[1], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var ts))
            {
                _logger.LogWarning("Invalid Timestamp in line: {Line}", line);
                continue;
            }

            if (!decimal.TryParse(parts[2], NumberStyles.Number, CultureInfo.InvariantCulture, out var consumption))
            {
                _logger.LogWarning("Invalid ConsumptionKwh in line: {Line}", line);
                continue;
            }

            var entity = new EnergyRecord
            {
                ApplianceId = applianceId,
                Timestamp = ts,
                ConsumptionKwh = consumption,
                IsActive = true
            };

            created.Add(entity);
        }

        if (created.Count == 0) return 0;

        await _db.EnergyRecords.AddRangeAsync(created, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return created.Count;
    }
}
