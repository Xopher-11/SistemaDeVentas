using SistemaDeVentas.Application.Models;

namespace SistemaDeVentas.Application.Interfaces
{
    public interface IFactSalesLoader
    {
        Task<WarehouseLoadSummary> LoadFactSalesAsync(
            string stagingFilePath,
            CancellationToken cancellationToken = default);
    }
}