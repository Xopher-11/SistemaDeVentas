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

        public async Task<DataExtractionResult> ExtractDataAsync(CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.Now;
            var timer = Stopwatch.StartNew();

            try
            {
                _logger.LogInfo("Starting API extraction...");

                var items = await _apiService.GetDataAsync(cancellationToken);
                var staging = await _stagingService.SaveToStagingAsync(items, "api_extraction.json", cancellationToken);

                timer.Stop();

                return new DataExtractionResult
                {
                    Source = DataSourceName,
                    Success = true,
                    ExtractedCount = items.Count,
                    Description = "API extraction completed successfully.",
                    StagingPath = staging.Path,
                    StartTime = startTime,
                    FinishTime = DateTime.Now,
                    DurationMs = timer.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                timer.Stop();
                _logger.LogFailure("API extraction failed.", ex);

                return new DataExtractionResult
                {
                    Source = DataSourceName,
                    Success = false,
                    ExtractedCount = 0,
                    Description = ex.Message,
                    StartTime = startTime,
                    FinishTime = DateTime.Now,
                    DurationMs = timer.ElapsedMilliseconds
                };
            }
        }
    }
}