namespace SistemaDeVentas.Application.SourceModels.API
{
    public class ShipmentTracking
    {
        public int TrackingId { get; set; }
        public int OrderId { get; set; }
        public string ShippingCompany { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public DateTime EstimatedArrival { get; set; }
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}