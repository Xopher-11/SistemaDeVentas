using SistemaDeVentas.Application.Interfaces;

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
            _logger.LogInformation("Iniciando el proceso de carga...");

            try
            {
                await LimpiarYcargarAsync(stoppingToken);
                _logger.LogInformation("Proceso terminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error durante el proceso.");
            }
            finally
            {
                _lifetime.StopApplication();
            }
        }

        private async Task LimpiarYcargarAsync(CancellationToken stoppingToken)
        {
            // Creamos un scope para poder usar los servicios Scoped
            using var scope = _scopeFactory.CreateScope();

            var cleaner = scope.ServiceProvider.GetRequiredService<IFactTableCleaner>();
            var factSalesLoader = scope.ServiceProvider.GetRequiredService<IFactSalesLoader>();

            _logger.LogInformation("Limpiando datos anteriores...");
            await cleaner.LimpiarFactSalesAsync(stoppingToken);

            _logger.LogInformation("Cargando registros de FactSales...");
            var result = await factSalesLoader.LoadFactSalesAsync(
                @"C:\Users\chris\Downloads\SistemaDeVentas\Staging", stoppingToken);

            if (result.WasSuccessful)
            {
                _logger.LogInformation("Registros cargados: {total}", result.TotalFactSales);
            }
            else
            {
                _logger.LogWarning("Hubo un problema en la carga: {msg}", result.Message);
            }
        }
    }
}