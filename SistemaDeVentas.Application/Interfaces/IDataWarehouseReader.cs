namespace SistemaDeVentas.Application.Interfaces
{
    public interface IDataWarehouseReader<T>
    {
        Task<List<T>> GetFromQueryAsync(string sqlQuery, CancellationToken cancellationToken = default);
    }
}