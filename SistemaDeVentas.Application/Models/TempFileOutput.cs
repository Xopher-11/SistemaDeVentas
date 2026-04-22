namespace SistemaDeVentas.Application.Models
{
    // Resultado de guardar los datos en el archivo de staging
    public class TempFileOutput
    {
        public bool WasCreated { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public int RecordsSaved { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}