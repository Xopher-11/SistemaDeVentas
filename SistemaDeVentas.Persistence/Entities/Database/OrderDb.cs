namespace SistemaDeVentas.Persistence.Entities.Database
{
    public class OrderDb
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public int StatusID { get; set; }
        public CustomerDb? Customer { get; set; }
        public OrderStatusDb? Status { get; set; }
        public ICollection<OrderDetailsDb> OrderDetails { get; set; } = new List<OrderDetailsDb>();
    }
}