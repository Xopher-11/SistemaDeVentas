using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaDeVentas.Application.Interfaces;
using SistemaDeVentas.Application.SourceModels.API;
using SistemaDeVentas.Application.SourceModels.CSV;
using SistemaDeVentas.Infrastructure.APIClients;
using SistemaDeVentas.Infrastructure.Extractors;
using SistemaDeVentas.Infrastructure.Readers;
using SistemaDeVentas.Infrastructure.Services;
using SistemaDeVentas.Infrastructure.Settings;
using SistemaDeVentas.Persistence.Loaders;

namespace SistemaDeVentas.Infrastructure.DependencyInjection
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Se cargan las configuraciones desde el appsettings
            services.Configure<CSVSettings>(configuration.GetSection("CSVSettings"));
            services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
            services.Configure<APISettings>(configuration.GetSection("APISettings"));
            services.Configure<StagingSettings>(configuration.GetSection("StagingSettings"));

            // Instancias directas (por simplicidad en el uso)
            var csvConfig = configuration.GetSection("CSVSettings").Get<CSVSettings>() ?? new CSVSettings();
            var dbConfig = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>() ?? new DatabaseSettings();
            var apiConfig = configuration.GetSection("APISettings").Get<APISettings>() ?? new APISettings();
            var stagingConfig = configuration.GetSection("StagingSettings").Get<StagingSettings>() ?? new StagingSettings();

            services.AddSingleton(csvConfig);
            services.AddSingleton(dbConfig);
            services.AddSingleton(apiConfig);
            services.AddSingleton(stagingConfig);

            // Lectores CSV (alineados con el modelo relacional del sistema)
            services.AddScoped<ICsvFileReader<CustomerRecord>, CSVFileService<CustomerRecord>>();
            services.AddScoped<ICsvFileReader<ProductRecord>, CSVFileService<ProductRecord>>();
            services.AddScoped<ICsvFileReader<OrderRecord>, CSVFileService<OrderRecord>>();
            services.AddScoped<ICsvFileReader<OrderDetailRecord>, CSVFileService<OrderDetailRecord>>();

            // Lector de base de datos
            services.AddScoped<IDataWarehouseReader<Dictionary<string, object?>>>(_ =>
                new SqlDatabaseReader(dbConfig.ConnectionString));

            // Cliente de API (seguimiento de órdenes)
            services.AddHttpClient<ISalesApiService<ShipmentTracking>, ShipmentTrackingService>();

            // Servicios generales del ETL
            services.AddScoped<IStagingAreaService, StagingFileService>();

            // Extractores (parte del proceso ETL hacia el DW)
            services.AddScoped<ISalesDataExtractor, CsvDataExtractor>();
            services.AddScoped<ISalesDataExtractor, DatabaseDataExtractor>();
            services.AddScoped<ISalesDataExtractor, ApiDataExtractor>();

            // servicios del proceso ETL
            services.AddScoped<IProcessLogger, ConsoleProcessLogger>();
            services.AddScoped<IDataWarehouseLoader, DataLoader>();
            services.AddScoped<IFactTableCleaner, FactTableCleaner>();
            services.AddScoped<IFactSalesLoader, FactSalesLoaderService>();

            return services;
        }
    }
}