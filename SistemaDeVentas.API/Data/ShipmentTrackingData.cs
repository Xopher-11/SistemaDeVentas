using SistemaDeVentas.API.Response;

namespace SistemaDeVentas.API.Data
{
    // datos simulados, el worker es el que usa esto para extraer la info
    public static class ShipmentTrackingData
    {
        public static List<ShipmentTrackingResponse> GetData()
        {
            var datos = new List<ShipmentTrackingResponse>
            {
                new ShipmentTrackingResponse
                {
                    TrackingId = 1,
                    OrderId = 1001,
                    ShippingCompany = "FedEx",
                    Status = "Delivered",
                    UpdatedAt = new DateTime(2024, 1, 15, 10, 30, 0),
                    EstimatedArrival = new DateTime(2024, 1, 15),
                    City = "New York",
                    Country = "United States"
                },
                new ShipmentTrackingResponse
                {
                    TrackingId = 2,
                    OrderId = 1002,
                    ShippingCompany = "DHL",
                    Status = "In Transit",
                    UpdatedAt = new DateTime(2024, 1, 16, 8, 0, 0),
                    EstimatedArrival = new DateTime(2024, 1, 18),
                    City = "Los Angeles",
                    Country = "United States"
                },
                new ShipmentTrackingResponse
                {
                    TrackingId = 3,
                    OrderId = 1003,
                    ShippingCompany = "UPS",
                    Status = "Pending",
                    UpdatedAt = new DateTime(2024, 1, 16, 14, 45, 0),
                    EstimatedArrival = new DateTime(2024, 1, 20),
                    City = "Chicago",
                    Country = "United States"
                },
                new ShipmentTrackingResponse
                {
                    TrackingId = 4,
                    OrderId = 1004,
                    ShippingCompany = "FedEx",
                    Status = "Delivered",
                    UpdatedAt = new DateTime(2024, 2, 3, 9, 15, 0),
                    EstimatedArrival = new DateTime(2024, 2, 3),
                    City = "Toronto",
                    Country = "Canada"
                },
                new ShipmentTrackingResponse
                {
                    TrackingId = 5,
                    OrderId = 1005,
                    ShippingCompany = "DHL",
                    Status = "In Transit",
                    UpdatedAt = new DateTime(2024, 2, 10, 11, 0, 0),
                    EstimatedArrival = new DateTime(2024, 2, 13),
                    City = "London",
                    Country = "United Kingdom"
                },
                new ShipmentTrackingResponse
                {
                    TrackingId = 6,
                    OrderId = 1006,
                    ShippingCompany = "Correos",
                    Status = "Delivered",
                    UpdatedAt = new DateTime(2024, 2, 20, 16, 0, 0),
                    EstimatedArrival = new DateTime(2024, 2, 20),
                    City = "Madrid",
                    Country = "Spain"
                },
                new ShipmentTrackingResponse
                {
                    TrackingId = 7,
                    OrderId = 1007,
                    ShippingCompany = "UPS",
                    Status = "Pending",
                    UpdatedAt = new DateTime(2024, 3, 5, 7, 30, 0),
                    EstimatedArrival = new DateTime(2024, 3, 8),
                    City = "Berlin",
                    Country = "Germany"
                },
                new ShipmentTrackingResponse
                {
                    TrackingId = 8,
                    OrderId = 1008,
                    ShippingCompany = "FedEx",
                    Status = "Delivered",
                    UpdatedAt = new DateTime(2024, 3, 12, 13, 20, 0),
                    EstimatedArrival = new DateTime(2024, 3, 12),
                    City = "Paris",
                    Country = "France"
                },
                new ShipmentTrackingResponse
                {
                    TrackingId = 9,
                    OrderId = 1009,
                    ShippingCompany = "DHL",
                    Status = "In Transit",
                    UpdatedAt = new DateTime(2024, 3, 18, 10, 0, 0),
                    EstimatedArrival = new DateTime(2024, 3, 21),
                    City = "Sydney",
                    Country = "Australia"
                },
                new ShipmentTrackingResponse
                {
                    TrackingId = 10,
                    OrderId = 1010,
                    ShippingCompany = "UPS",
                    Status = "Delivered",
                    UpdatedAt = new DateTime(2024, 4, 1, 15, 45, 0),
                    EstimatedArrival = new DateTime(2024, 4, 1),
                    City = "Tokyo",
                    Country = "Japan"
                }
            };

            return datos;
        }
    }
}