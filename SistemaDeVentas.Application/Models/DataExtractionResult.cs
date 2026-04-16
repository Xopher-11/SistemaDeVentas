namespace SistemaDeVentas.Application.Models
{
    public class DataExtractionResult
    {
        public string Source { get; set; } = string.Empty;
        public bool Success { get; set; }
        public int ExtractedCount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? StagingPath { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public long DurationMs { get; set; }
    }
}