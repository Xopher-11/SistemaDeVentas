namespace SistemaDeVentas.Application.Interfaces
{
    public interface ISalesApiService<T>
    {
        Task<List<T>> GetDataAsync(CancellationToken cancellationToken = default);
    }
}