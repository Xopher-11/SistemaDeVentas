namespace SistemaDeVentas.Persistence.Entities.Database
{
    public class OrderStatusDb
    {
        public int StatusID { get; set; }
        public string Status { get; set; } = string.Empty;
        public ICollection<OrderDb> Orders { get; set; } = new List<OrderDb>();
    }
}