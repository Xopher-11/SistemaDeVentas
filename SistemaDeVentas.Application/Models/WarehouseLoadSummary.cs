namespace SistemaDeVentas.Application.Models
{
    // Resultado del proceso de carga a la base de datos dimensional
    public class WarehouseLoadSummary
    {
        public bool WasSuccessful { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        // Totales de cada tabla cargada
        public int TotalCategories { get; set; }
        public int TotalProducts { get; set; }
        public int TotalLocations { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalStatuses { get; set; }
        public int TotalDates { get; set; }
        public int TotalFactSales { get; set; }
    }
}