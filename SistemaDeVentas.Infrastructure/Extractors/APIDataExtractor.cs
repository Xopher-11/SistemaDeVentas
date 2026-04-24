using SistemaDeVentas.Application.Interfaces;
using SistemaDeVentas.Application.Models;
using SistemaDeVentas.Application.SourceModels.API;
using System.Diagnostics;

namespace SistemaDeVentas.Infrastructure.Extractors
{
    public class ApiDataExtractor : ISalesDataExtractor
    {
        private readonly ISalesApiService<ShipmentTracking> _apiService;
        private readonly IStagingAreaService _stagingService;
        private readonly IProcessLogger _logger;

        public string DataSourceName => "REST API";

        public ApiDataExtractor(
            ISalesApiService<ShipmentTracking> apiService,
            IStagingAreaService stagingService,
            IProcessLogger logger)
        {
            _apiService = apiService;
            _stagingService = stagingService;
            _logger = logger;
        }

        public async Task<SourceExtractionInfo> ExtractDataAsync(CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.Now;
            var timer = Stopwatch.StartNew();

            try
            {
                _logger.LogInfo("Iniciando extracción de API...");

                var items = await _apiService.GetDataAsync(cancellationToken);
                var staging = await _stagingService.SaveToStagingAsync(
                    items,
                    "api_extraction.json",
                    cancellationToken);

                timer.Stop();

                return new SourceExtractionInfo
                {
                    SourceName = DataSourceName,
                    WasSuccessful = true,
                    RecordsExtracted = items.Count,
                    Message = "La extracción de la API se completó con éxito.",
                    StagingFilePath = staging.FilePath,
                    StartedAt = startTime,
                    EndedAt = DateTime.Now,
                    DurationInMilliseconds = timer.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                timer.Stop();
                _logger.LogFailure("La extracción de API falló.", ex);

                return new SourceExtractionInfo
                {
                    SourceName = DataSourceName,
                    WasSuccessful = false,
                    RecordsExtracted = 0,
                    Message = ex.Message,
                    StartedAt = startTime,
                    EndedAt = DateTime.Now,
                    DurationInMilliseconds = timer.ElapsedMilliseconds
                };
            }
        }
    }
}