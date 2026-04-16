namespace SistemaDeVentas.Application.SourceModels.CSV
{
    public class ProductRecord
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int StockAvailable { get; set; }
    }
}