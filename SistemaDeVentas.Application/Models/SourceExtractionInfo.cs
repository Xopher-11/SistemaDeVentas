namespace SistemaDeVentas.Application.Models
{
    // Guarda el resultado de extraer datos de una fuente
    public class SourceExtractionInfo
    {
        public string SourceName { get; set; } = string.Empty;
        public bool WasSuccessful { get; set; }
        public int RecordsExtracted { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? StagingFilePath { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public long DurationInMilliseconds { get; set; }
    }
}