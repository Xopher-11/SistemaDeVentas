namespace SistemaDeVentas.Persistence.Entities.DataWareHouse.Dimensions
{
    public class DimLocation
    {
        public int LocationKey { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public ICollection<DimCustomer> Customers { get; set; } = new List<DimCustomer>();
        public ICollection<Facts.FactSales> FactSales { get; set; } = new List<Facts.FactSales>();
    }
}