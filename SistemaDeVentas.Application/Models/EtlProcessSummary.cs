namespace SistemaDeVentas.Application.Models
{
    public class EtlProcessSummary
    {
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public int SourcesProcessed { get; set; }
        public int TotalRows { get; set; }
        public long TotalTimeMs { get; set; }
        public List<DataExtractionResult> ExtractionDetails { get; set; } = [];
    }
}