namespace SistemaDeVentas.API.Response
{
    // modelo que devuelve el endpoint, debe tener los mismos campos que ShipmentTracking
    public class ShipmentTrackingResponse
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