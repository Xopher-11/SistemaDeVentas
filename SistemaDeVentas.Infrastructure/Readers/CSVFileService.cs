using CsvHelper.Configuration;
using SistemaDeVentas.Application.Interfaces;
using System.Globalization;

namespace SistemaDeVentas.Infrastructure.Readers
{
    public class CSVFileService<T> : ICsvFileReader<T>
    {
        // Se encarga de leer un archivo CSV y convertirlo en una lista del tipo T
        public async Task<List<T>> ReadFileAsync(string path, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File not found at path: {path}");
            }

            var dataList = new List<T>();

            using var fileReader = new StreamReader(path);

            // Configuración básica del CSV (evita errores por campos faltantes)
            using var csvReader = new CsvHelper.CsvReader(fileReader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            });

            // Se recorren los registros del archivo
            await foreach (var item in csvReader.GetRecordsAsync<T>(cancellationToken))
            {
                dataList.Add(item);
            }

            return dataList;
        }
    }
}