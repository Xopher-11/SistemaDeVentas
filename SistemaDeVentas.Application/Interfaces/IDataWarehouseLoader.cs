namespace SistemaDeVentas.Application.Interfaces
{
    public interface IDataWarehouseLoader
    {
        Task LoadDataAsync<T>(IEnumerable<T> records, CancellationToken cancellationToken = default) where T : class;
    }
}