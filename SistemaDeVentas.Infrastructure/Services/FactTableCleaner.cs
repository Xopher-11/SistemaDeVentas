using Microsoft.EntityFrameworkCore;
using SistemaDeVentas.Application.Interfaces;
using SistemaDeVentas.Persistence.Context;

namespace SistemaDeVentas.Infrastructure.Services
{
    public class FactTableCleaner : IFactTableCleaner
    {
        private readonly SalesDataWarehouseContext _context;
        private readonly IProcessLogger _logger;

        public FactTableCleaner(SalesDataWarehouseContext context, IProcessLogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LimpiarFactSalesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInfo("Limpiando tabla FactSales...");

            // borramos todo antes de volver a cargar
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM FactSales", cancellationToken);

            _logger.LogInfo("FactSales limpiada correctamente");
        }
    }
}