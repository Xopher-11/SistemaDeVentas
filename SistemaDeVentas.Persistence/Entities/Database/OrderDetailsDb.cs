namespace SistemaDeVentas.Persistence.Entities.Database
{
    public class OrderDetailsDb
    {
        public int OrderDetailsID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public OrderDb? Order { get; set; }
        public ProductDb? Product { get; set; }
    }
}