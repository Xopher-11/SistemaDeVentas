using SistemaDeVentas.Persistence.Context;
using SistemaDeVentas.Application.Interfaces;

namespace SistemaDeVentas.Persistence.Entities.Loaders
{
    public class DataLoader : IDataWarehouseLoader
    {
        private readonly SalesDataWarehouseContext _warehouseContext;

        public DataLoader(SalesDataWarehouseContext warehouseContext)
        {
            _warehouseContext = warehouseContext;
        }

        // Carga una colección de datos en la base indicada
        public async Task LoadDataAsync<T>(IEnumerable<T> data, CancellationToken cancellationToken = default) where T : class
        {
            if (data == null || !data.Any())
            {
                return;
            }

            await _warehouseContext.Set<T>().AddRangeAsync(data, cancellationToken);
            await _warehouseContext.SaveChangesAsync(cancellationToken);
        }
    }
}