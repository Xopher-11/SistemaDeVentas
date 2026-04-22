using SistemaDeVentas.Application.Models;

namespace SistemaDeVentas.Application.Interfaces
{
    public interface ISalesDataExtractor
    {
        string DataSourceName { get; }
        Task<SourceExtractionInfo> ExtractDataAsync(CancellationToken cancellationToken = default);
    }
}