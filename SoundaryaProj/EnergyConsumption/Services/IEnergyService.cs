using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SoundaryaProj.EnergyConsumption.Models;

namespace SoundaryaProj.EnergyConsumption.Services;

public interface IEnergyService
{
    Task<EnergyRecord> CreateAsync(CreateEnergyDto dto, CancellationToken cancellationToken);
    Task<PagedResult<EnergyRecord>> GetAllAsync(int? applianceId, int page, int pageSize, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken);
    Task<EnergyRecord?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task UpdateAsync(int id, UpdateEnergyDto dto, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<EnergyRecord>> GetByApplianceAsync(int applianceId, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken);
    Task<int> ImportCsvAsync(Stream csvStream, CancellationToken cancellationToken);
}
