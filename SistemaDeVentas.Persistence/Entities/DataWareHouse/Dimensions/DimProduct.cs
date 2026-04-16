namespace SistemaDeVentas.Persistence.Entities.DataWareHouse.Dimensions
{
    public class DimProduct
    {
        public int ProductKey { get; set; }
        public int ProductID_NaturalKey { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int? CategoryKey { get; set; }
        public decimal ListPrice { get; set; }
        public int Stock { get; set; }
        public DimCategory? Category { get; set; }
        public ICollection<Facts.FactSales> FactSales { get; set; } = new List<Facts.FactSales>();
    }
}