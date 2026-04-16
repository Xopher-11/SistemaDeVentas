using SistemaDeVentas.Application.Interfaces;
using SistemaDeVentas.Application.Models;
using System.Diagnostics;

namespace SistemaDeVentas.Application.Services
{
    public class EtlCoordinator
    {
        private readonly IEnumerable<ISalesDataExtractor> _dataExtractors;
        private readonly IProcessLogger _logger;

        public EtlCoordinator(IEnumerable<ISalesDataExtractor> dataExtractors, IProcessLogger logger)
        {
            _dataExtractors = dataExtractors;
            _logger = logger;
        }

        public async Task<EtlProcessSummary> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var summary = new EtlProcessSummary
            {
                StartTime = DateTime.Now
            };

            var timer = Stopwatch.StartNew();

            _logger.LogInfo("ETL process started...");

            foreach (var extractor in _dataExtractors)
            {
                try
                {
                    _logger.LogInfo($"Processing source: {extractor.DataSourceName}");

                    var extractionResult = await extractor.ExtractDataAsync(cancellationToken);

                    summary.ExtractionDetails.Add(extractionResult);
                    summary.SourcesProcessed++;
                    summary.TotalRows += extractionResult.ExtractedCount;

                    _logger.LogInfo($"Source completed: {extractor.DataSourceName}");
                }
                catch (Exception ex)
                {
                    _logger.LogFailure($"Failure processing source: {extractor.DataSourceName}", ex);

                    // Guardamos el fallo para tener trazabilidad en el resumen
                    summary.ExtractionDetails.Add(new DataExtractionResult
                    {
                        Source = extractor.DataSourceName,
                        Success = false,
                        ExtractedCount = 0,
                        Description = ex.Message,
                        StartTime = DateTime.Now,
                        FinishTime = DateTime.Now,
                        DurationMs = 0
                    });

                    summary.SourcesProcessed++;
                }
            }

            timer.Stop();

            summary.FinishTime = DateTime.Now;
            summary.TotalTimeMs = timer.ElapsedMilliseconds;

            _logger.LogInfo("ETL process finished.");

            return summary;
        }
    }
}