using SistemaDeVentas.Application.Models;

namespace SistemaDeVentas.Application.Interfaces
{
    public interface IFactSalesLoader
    {
        Task<WarehouseLoadSummary> ProcessDimensionsAsync(
            string stagingFilePath,
            CancellationToken cancellationToken = default);

        Task<WarehouseLoadSummary> LoadFactSalesAsync(
            string stagingFilePath,
            CancellationToken cancellationToken = default);
    }
}