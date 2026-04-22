namespace SistemaDeVentas.Application.Interfaces
{
    // interfaz para limpiar las tablas fact antes de cargar
    public interface IFactTableCleaner
    {
        Task LimpiarFactSalesAsync(CancellationToken cancellationToken = default);
    }
}