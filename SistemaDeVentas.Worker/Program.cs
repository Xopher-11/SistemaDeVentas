using Microsoft.EntityFrameworkCore;
using SistemaDeVentas.Application.Services;
using SistemaDeVentas.Infrastructure.DependencyInjection;
using SistemaDeVentas.Persistence.Context;

namespace SistemaDeVentas.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddHostedService<Worker>();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddScoped<EtlCoordinator>();
            builder.Services.AddDbContextPool<SalesDataWarehouseContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DataWarehouseConnection")));

            var host = builder.Build();
            host.Run();
        }
    }
}