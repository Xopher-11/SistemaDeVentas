namespace SistemaDeVentas.Persistence.Entities.Database
{
    public class CategoryDb
    {
        public int CategoryID { get; set; }
        public string Category { get; set; } = string.Empty;

        public ICollection<ProductDb> Products { get; set; } = new List<ProductDb>();
    }
}