namespace SistemaDeVentas.Infrastructure.Settings
{
    public class CSVSettings
    {
        public string BasePath { get; set; } = string.Empty;
        public string CustomersFileName { get; set; } = string.Empty;
        public string ProductsFileName { get; set; } = string.Empty;
        public string OrdersFileName { get; set; } = string.Empty;
        public string OrderDetailsFileName { get; set; } = string.Empty;
    }
}