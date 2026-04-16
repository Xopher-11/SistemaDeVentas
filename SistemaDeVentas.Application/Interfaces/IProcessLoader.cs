namespace SistemaDeVentas.Application.Interfaces
{
    public interface IProcessLogger
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogFailure(string message, Exception? exception = null);
    }
}