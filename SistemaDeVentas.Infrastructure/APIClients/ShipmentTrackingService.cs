using System.Net.Http.Json;
using SistemaDeVentas.Application.Interfaces;
using SistemaDeVentas.Application.SourceModels.API;
using SistemaDeVentas.Infrastructure.Settings;

namespace SistemaDeVentas.Infrastructure.APIClients
{
    public class ShipmentTrackingService : ISalesApiService<ShipmentTracking>
    {
        private readonly HttpClient _client;
        private readonly APISettings _settings;

        public ShipmentTrackingService(HttpClient client, APISettings settings)
        {
            _client = client;
            _settings = settings;
        }

        // Obtiene los datos de seguimiento de órdenes desde la API externa
        public async Task<List<ShipmentTracking>> GetDataAsync(CancellationToken cancellationToken = default)
        {
            var url = $"{_settings.BaseUrl.TrimEnd('/')}/{_settings.ShipmentTrackingEndpoint.TrimStart('/')}";

            var httpResponse = await _client.GetAsync(url, cancellationToken);
            httpResponse.EnsureSuccessStatusCode();

            var result = await httpResponse.Content.ReadFromJsonAsync<List<ShipmentTracking>>(cancellationToken: cancellationToken);

            return result ?? new List<ShipmentTracking>();
        }
    }
}