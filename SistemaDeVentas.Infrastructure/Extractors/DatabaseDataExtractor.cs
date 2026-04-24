using Microsoft.Extensions.Options;
using SistemaDeVentas.Application.Interfaces;
using SistemaDeVentas.Application.Models;
using SistemaDeVentas.Infrastructure.Settings;
using System.Diagnostics;

namespace SistemaDeVentas.Infrastructure.Extractors
{
    public class DatabaseDataExtractor : ISalesDataExtractor
    {
        private readonly IDataWarehouseReader<Dictionary<string, object?>> _databaseReader;
        private readonly IStagingAreaService _stagingService;
        private readonly IProcessLogger _logger;
        private readonly DatabaseSettings _databaseSettings;

        public string DataSourceName => "Relational Database";

        public DatabaseDataExtractor(
            IDataWarehouseReader<Dictionary<string, object?>> databaseReader,
            IStagingAreaService stagingService,
            IProcessLogger logger,
            IOptions<DatabaseSettings> databaseOptions)
        {
            _databaseReader = databaseReader;
            _stagingService = stagingService;
            _logger = logger;
            _databaseSettings = databaseOptions.Value;
        }

        public async Task<SourceExtractionInfo> ExtractDataAsync(CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.Now;
            var timer = Stopwatch.StartNew();

            try
            {
                _logger.LogInfo("Iniciando extracción de base de datos...");

                var rows = await _databaseReader.GetFromQueryAsync(
                    _databaseSettings.ExtractionSql,
                    cancellationToken);

                var staging = await _stagingService.SaveToStagingAsync(
                    rows,
                    "dw_extraction.json",
                    cancellationToken);

                timer.Stop();

                return new SourceExtractionInfo
                {
                    SourceName = DataSourceName,
                    WasSuccessful = true,
                    RecordsExtracted = rows.Count,
                    Message = "La extracción de la base de datos se completó con éxito.",
                    StagingFilePath = staging.FilePath,
                    StartedAt = startTime,
                    EndedAt = DateTime.Now,
                    DurationInMilliseconds = timer.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                timer.Stop();
                _logger.LogFailure("Error en la extracción de la base de datos.", ex);

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