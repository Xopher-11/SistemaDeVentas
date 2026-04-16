using Microsoft.Data.SqlClient;
using SistemaDeVentas.Application.Interfaces;

namespace SistemaDeVentas.Infrastructure.Readers
{
    public class SqlDatabaseReader : IDataWarehouseReader<Dictionary<string, object?>>
    {
        private readonly string _connectionString;

        public SqlDatabaseReader(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Ejecuta una consulta y devuelve los datos en formato clave-valor
        public async Task<List<Dictionary<string, object?>>> GetFromQueryAsync(
            string sqlQuery,
            CancellationToken cancellationToken = default)
        {
            var resultList = new List<Dictionary<string, object?>>();

            await using var dbConnection = new SqlConnection(_connectionString);
            await dbConnection.OpenAsync(cancellationToken);

            await using var dbCommand = new SqlCommand(sqlQuery, dbConnection);
            await using var dbReader = await dbCommand.ExecuteReaderAsync(cancellationToken);

            // Se leen todas las filas del resultado
            while (await dbReader.ReadAsync(cancellationToken))
            {
                var rowData = new Dictionary<string, object?>();

                for (int i = 0; i < dbReader.FieldCount; i++)
                {
                    var columnName = dbReader.GetName(i);
                    var value = dbReader.IsDBNull(i) ? null : dbReader.GetValue(i);

                    rowData[columnName] = value;
                }

                resultList.Add(rowData);
            }

            return resultList;
        }
    }
}