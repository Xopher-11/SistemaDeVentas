namespace SistemaDeVentas.Persistence.Entities.Database
{
    public class ProductDb
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int? CategoryID { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public CategoryDb? Category { get; set; }
        public ICollection<OrderDetailsDb> OrderDetails { get; set; } = new List<OrderDetailsDb>();
    }
}