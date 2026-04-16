using SistemaDeVentas.Application.Interfaces;
using SistemaDeVentas.Application.Models;
using SistemaDeVentas.Infrastructure.Settings;
using System.Collections;
using System.Text.Json;

namespace SistemaDeVentas.Infrastructure.Services
{
    public class StagingFileService : IStagingAreaService
    {
        private readonly StagingSettings _settings;

        public StagingFileService(StagingSettings settings)
        {
            _settings = settings;
        }

        // Guarda los datos extraídos en formato JSON dentro del área de staging
        public async Task<StagingResult> SaveToStagingAsync<T>(
            T data,
            string baseFileName,
            CancellationToken cancellationToken = default)
        {
            // Verifica que exista la carpeta de staging
            if (!Directory.Exists(_settings.FolderPath))
            {
                Directory.CreateDirectory(_settings.FolderPath);
            }

            // Genera un nombre único con timestamp
            var fileName = $"{Path.GetFileNameWithoutExtension(baseFileName)}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var fullPath = Path.Combine(_settings.FolderPath, fileName);

            // Serializa los datos a JSON (formato legible)
            var jsonContent = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(fullPath, jsonContent, cancellationToken);

            // Calcula la cantidad de registros (si es colección)
            int totalRecords = 1;

            if (data is IEnumerable collection && data is not string)
            {
                totalRecords = 0;
                foreach (var _ in collection)
                {
                    totalRecords++;
                }
            }

            // Retorna resultado del proceso de staging
            return new StagingResult
            {
                Created = true,
                Path = fullPath,
                SavedRecords = totalRecords,
                Details = "Data successfully stored in staging area."
            };
        }
    }
}