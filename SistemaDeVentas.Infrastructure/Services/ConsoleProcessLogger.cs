using SistemaDeVentas.Application.Interfaces;

namespace SistemaDeVentas.Infrastructure.Services
{
    public class ConsoleProcessLogger : IProcessLogger
    {
        public void LogInfo(string message) =>
            Console.WriteLine($"[INFO]  {DateTime.Now:HH:mm:ss} - {message}");

        public void LogWarning(string message) =>
            Console.WriteLine($"[ADVERTENCIA]  {DateTime.Now:HH:mm:ss} - {message}");

        public void LogFailure(string message, Exception? exception = null)
        {
            Console.WriteLine($"[ERROR] {DateTime.Now:HH:mm:ss} - {message}");
            if (exception != null)
                Console.WriteLine($"        Excepción: {exception.Message}");
        }
    }
}