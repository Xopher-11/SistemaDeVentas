namespace SistemaDeVentas.Persistence.Entities.DataWareHouse.Dimensions
{
    public class DimOrderStatus
    {
        public int StatusKey { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public ICollection<Facts.FactSales> FactSales { get; set; } = new List<Facts.FactSales>();
    }
}