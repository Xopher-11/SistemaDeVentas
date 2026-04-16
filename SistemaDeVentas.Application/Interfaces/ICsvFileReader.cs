namespace SistemaDeVentas.Application.Interfaces
{
    public interface ICsvFileReader<T>
    {
        Task<List<T>> ReadFileAsync(string filePath, CancellationToken cancellationToken = default);
    }
}