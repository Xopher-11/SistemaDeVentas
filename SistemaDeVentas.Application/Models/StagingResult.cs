namespace SistemaDeVentas.Application.Models
{
    public class StagingResult
    {
        public bool Created { get; set; }
        public string Path { get; set; } = string.Empty;
        public int SavedRecords { get; set; }
        public string Details { get; set; } = string.Empty;
    }
}