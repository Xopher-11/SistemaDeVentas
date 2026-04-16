using SistemaDeVentas.Application.Models;

namespace SistemaDeVentas.Application.Interfaces
{
    public interface IStagingAreaService
    {
        Task<StagingResult> SaveToStagingAsync<T>(T data, string fileName, CancellationToken cancellationToken = default);
    }
}