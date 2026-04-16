namespace SistemaDeVentas.Persistence.Entities.DataWareHouse.Dimensions
{
    public class DimCustomer
    {
        public int CustomerKey { get; set; }
        public int CustomerID_NaturalKey { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? LocationKey { get; set; }
        public DimLocation? Location { get; set; }
        public ICollection<Facts.FactSales> FactSales { get; set; } = new List<Facts.FactSales>();
    }
}