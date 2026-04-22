namespace SistemaDeVentas.Application.Models
{
    // Métricas generales del proceso ETL completo
    public class PipelineRunStats
    {
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public int TotalSources { get; set; }
        public int TotalRecords { get; set; }
        public long TotalDurationInMilliseconds { get; set; }

        // Lista con el detalle de cada extracción hecha
        public List<SourceExtractionInfo> Results { get; set; } = new List<SourceExtractionInfo>();
    }
}