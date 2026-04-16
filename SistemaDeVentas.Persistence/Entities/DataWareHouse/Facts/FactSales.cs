using SistemaDeVentas.Persistence.Entities.DataWareHouse.Dimensions;

namespace SistemaDeVentas.Persistence.Entities.DataWareHouse.Facts
{
    public class FactSales
    {
        public long SalesKey { get; set; }
        public int DateKey { get; set; }
        public int CustomerKey { get; set; }
        public int ProductKey { get; set; }
        public int? LocationKey { get; set; }
        public int StatusKey { get; set; }
        public int OrderID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SalesAmount { get; set; }
        public DimDate? Date { get; set; }
        public DimCustomer? Customer { get; set; }
        public DimProduct? Product { get; set; }
        public DimLocation? Location { get; set; }
        public DimOrderStatus? Status { get; set; }
    }
}