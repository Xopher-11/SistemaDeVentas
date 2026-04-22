using Microsoft.AspNetCore.Mvc;
using SistemaDeVentas.API.Data;
using SistemaDeVentas.API.Response;

namespace SistemaDeVentas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentTrackingController : ControllerBase
    {
        // GET api/ShipmentTracking
        // el worker llama este endpoint para obtener los datos de seguimiento
        [HttpGet]
        public ActionResult<List<ShipmentTrackingResponse>> GetAll()
        {
            var datos = ShipmentTrackingData.GetData();

            if (datos == null || datos.Count == 0)
                return NotFound("No hay datos de seguimiento disponibles");

            return Ok(datos);
        }

        // GET api/ShipmentTracking/{id}
        [HttpGet("{id}")]
        public ActionResult<ShipmentTrackingResponse> GetById(int id)
        {
            var datos = ShipmentTrackingData.GetData();
            var tracking = datos.FirstOrDefault(t => t.TrackingId == id);

            if (tracking == null)
                return NotFound($"No se encontro el tracking con id {id}");

            return Ok(tracking);
        }
    }
}