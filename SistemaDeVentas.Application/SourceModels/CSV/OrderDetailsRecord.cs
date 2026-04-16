namespace SistemaDeVentas.Application.SourceModels.CSV
{
    public class OrderDetailRecord
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}