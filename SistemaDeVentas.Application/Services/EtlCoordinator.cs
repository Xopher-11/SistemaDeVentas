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

        public async Task<PipelineRunStats> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var summary = new PipelineRunStats
            {
                StartedAt = DateTime.Now
            };

            var timer = Stopwatch.StartNew();

            _logger.LogInfo("ETL process started...");

            foreach (var extractor in _dataExtractors)
            {
                try
                {
                    _logger.LogInfo($"Processing source: {extractor.DataSourceName}");

                    var extractionResult = await extractor.ExtractDataAsync(cancellationToken);

                    summary.Results.Add(extractionResult);
                    summary.TotalSources++;
                    summary.TotalRecords += extractionResult.RecordsExtracted;

                    _logger.LogInfo($"Source completed: {extractor.DataSourceName}");
                }
                catch (Exception ex)
                {
                    _logger.LogFailure($"Failure processing source: {extractor.DataSourceName}", ex);

                    // Guardamos el fallo para tener trazabilidad en el resumen
                    summary.Results.Add(new SourceExtractionInfo
                    {
                        SourceName = extractor.DataSourceName,
                        WasSuccessful = false,
                        RecordsExtracted = 0,
                        Message = ex.Message,
                        StartedAt = DateTime.Now,
                        EndedAt = DateTime.Now,
                        DurationInMilliseconds = 0
                    });

                    summary.TotalSources++;
                }
            }

            timer.Stop();

            summary.EndedAt = DateTime.Now;
            summary.TotalDurationInMilliseconds = timer.ElapsedMilliseconds;

            _logger.LogInfo("ETL process finished.");

            return summary;
        }
    }
}