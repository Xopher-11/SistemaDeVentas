namespace SistemaDeVentas.Application.SourceModels.CSV
{
    public class OrderRecord
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime DateCreated { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
    }
}