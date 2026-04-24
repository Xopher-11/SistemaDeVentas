using SistemaDeVentas.Application.Interfaces;
using SistemaDeVentas.Application.Services;

namespace SistemaDeVentas.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHostApplicationLifetime _lifetime;

        public Worker(
            ILogger<Worker> logger,
            IServiceScopeFactory scopeFactory,
            IHostApplicationLifetime lifetime)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _lifetime = lifetime;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Iniciando proceso ETL completo...");

            try
            {
                using var scope = _scopeFactory.CreateScope();

                var etlCoordinator = scope.ServiceProvider.GetRequiredService<EtlCoordinator>();
                var factSalesLoader = scope.ServiceProvider.GetRequiredService<IFactSalesLoader>();

                var pipelineResult = await etlCoordinator.ExecuteAsync(stoppingToken);

                foreach (var item in pipelineResult.Results)
                {
                    _logger.LogInformation(
                        "Fuente: {source} | Exito: {ok} | Registros: {records} | Mensaje: {msg} | Archivo: {file}",
                        item.SourceName,
                        item.WasSuccessful,
                        item.RecordsExtracted,
                        item.Message,
                        item.StagingFilePath ?? "sin archivo");
                }

                var csvResult = pipelineResult.Results
                    .FirstOrDefault(r =>
                        r.WasSuccessful &&
                        !string.IsNullOrWhiteSpace(r.StagingFilePath) &&
                        r.SourceName.Contains("CSV", StringComparison.OrdinalIgnoreCase));

                var csvFailure = pipelineResult.Results
                    .FirstOrDefault(r => r.SourceName.Contains("CSV", StringComparison.OrdinalIgnoreCase));

                if (csvResult is null || string.IsNullOrWhiteSpace(csvResult.StagingFilePath))
                {
                    throw new InvalidOperationException(
                        $"Falló la extracción CSV. Detalle: {csvFailure?.Message ?? "No se encontró el archivo de staging del extractor CSV."}");
                }

                var csvStagingFile = csvResult.StagingFilePath;

                _logger.LogInformation("Archivo CSV de staging encontrado: {file}", csvStagingFile);

                _logger.LogInformation("Procesando dimensiones...");
                var dimensionsResult = await factSalesLoader.ProcessDimensionsAsync(csvStagingFile, stoppingToken);

                if (!dimensionsResult.WasSuccessful)
                {
                    throw new InvalidOperationException(
                        $"Error al cargar dimensiones: {dimensionsResult.Message}");
                }

                _logger.LogInformation("Procesando FactSales...");
                var factResult = await factSalesLoader.LoadFactSalesAsync(csvStagingFile, stoppingToken);

                if (!factResult.WasSuccessful)
                {
                    throw new InvalidOperationException(
                        $"Error al cargar FactSales: {factResult.Message}");
                }

                _logger.LogInformation("Proceso ETL finalizado correctamente.");
                _logger.LogInformation("La aplicación seguirá abierta hasta que la detengas manualmente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error durante el proceso ETL.");
            }

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Aplicación detenida manualmente.");
            }
        }
    }
}