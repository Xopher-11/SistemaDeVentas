namespace SistemaDeVentas.Application.SourceModels.CSV
{
    public class CustomerRecord
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
    }
}