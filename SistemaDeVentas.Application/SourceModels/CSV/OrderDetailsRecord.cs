using System.Text.Json.Serialization;

namespace SistemaDeVentas.Application.SourceModels.CSV
{
    public class OrderDetailRecord
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}