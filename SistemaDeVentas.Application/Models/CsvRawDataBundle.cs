using SistemaDeVentas.Application.SourceModels.CSV;

namespace SistemaDeVentas.Application.Models
{
    // Clase que guarda los datos extraídos del CSV antes de procesarlos
    public class CsvRawDataBundle
    {
        public List<CustomerRecord> CustomerList { get; set; } = new List<CustomerRecord>();
        public List<ProductRecord> ProductList { get; set; } = new List<ProductRecord>();
        public List<OrderRecord> OrderList { get; set; } = new List<OrderRecord>();
        public List<OrderDetailRecord> OrderDetailList { get; set; } = new List<OrderDetailRecord>();
    }
}