using Microsoft.Extensions.Logging;
using SistemaDeVentas.Application.Interfaces;

namespace SistemaDeVentas.Infrastructure.Services
{
    public class ProcessLogger : IProcessLogger
    {
        private readonly ILogger<ProcessLogger> _logger;

        public ProcessLogger(ILogger<ProcessLogger> logger)
        {
            _logger = logger;
        }

        // Mensajes informativos (flujo normal)
        public void LogInfo(string message)
        {
            _logger.LogInformation(message);
        }

        // Advertencias (algo no crítico)
        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }

        // Errores del sistema
        public void LogFailure(string message, Exception? ex = null)
        {
            _logger.LogError(ex, message);
        }
    }
}