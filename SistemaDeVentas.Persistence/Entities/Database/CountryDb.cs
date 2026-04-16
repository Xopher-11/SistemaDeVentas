namespace SistemaDeVentas.Persistence.Entities.Database
{
    public class CountryDb
    {
        public int CountryID { get; set; }
        public string Country { get; set; } = string.Empty;

        public ICollection<CityDb> Cities { get; set; } = new List<CityDb>();
    }
}