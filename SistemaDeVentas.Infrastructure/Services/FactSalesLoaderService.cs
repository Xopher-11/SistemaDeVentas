using Microsoft.EntityFrameworkCore;
using SistemaDeVentas.Application.Interfaces;
using SistemaDeVentas.Application.Models;
using SistemaDeVentas.Application.SourceModels.CSV;
using SistemaDeVentas.Persistence.Context;
using SistemaDeVentas.Persistence.Entities.DataWareHouse.Dimensions;
using SistemaDeVentas.Persistence.Entities.DataWareHouse.Facts;
using System.Text.Json;
using SistemaDeVentas.Application.Utils;

namespace SistemaDeVentas.Infrastructure.Services
{
    public class FactSalesLoaderService : IFactSalesLoader
    {
        private readonly SalesDataWarehouseContext _dwContext;
        private readonly IProcessLogger _logger;

        public FactSalesLoaderService(SalesDataWarehouseContext dwContext, IProcessLogger logger)
        {
            _dwContext = dwContext;
            _logger = logger;
        }


        // Procesa y carga todas las dimensiones al Data Warehouse
        public async Task<WarehouseLoadSummary> ProcessDimensionsAsync(string stagingFilePath, CancellationToken cancellationToken = default)
        {
            var result = new WarehouseLoadSummary
            {
                ProcessName = "Process Dimensions"
            };

            try
            {
                _logger.LogInfo("Starting dimension processing...");

                var stagingData = await LoadStagingFileAsync(stagingFilePath, cancellationToken);

                await using var transaction = await _dwContext.Database.BeginTransactionAsync(cancellationToken);

                await DeleteFactSalesAsync(cancellationToken);
                await DeleteDimensionsAsync(cancellationToken);

                // Cargamos cada dimensión por separado
                result.TotalCategories = await LoadCategoriesAsync(stagingData.ProductList, cancellationToken);
                result.TotalProducts = await LoadProductsAsync(stagingData.ProductList, cancellationToken);
                result.TotalLocations = await LoadLocationsAsync(stagingData.CustomerList, cancellationToken);
                result.TotalCustomers = await LoadCustomersAsync(stagingData.CustomerList, cancellationToken);
                result.TotalStatuses = await LoadOrderStatusesAsync(stagingData.OrderList, cancellationToken);
                result.TotalDates = await LoadDatesAsync(stagingData.OrderList, cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                result.WasSuccessful = true;
                result.Message = "Dimensions processed and loaded successfully.";

                _logger.LogInfo("Dimension processing completed.");
            }
            catch (Exception ex)
            {
                result.WasSuccessful = false;
                result.Message = ex.Message;

                _logger.LogFailure("Error while processing dimensions.", ex);
            }

            return result;
        }


        // Procesa y carga la tabla de hechos FactSales
        public async Task<WarehouseLoadSummary> LoadFactSalesAsync(string stagingFilePath, CancellationToken cancellationToken = default)
        {
            var result = new WarehouseLoadSummary
            {
                ProcessName = "Process FactSales"
            };

            try
            {
                _logger.LogInfo("Starting FactSales processing...");

                var stagingData = await LoadStagingFileAsync(stagingFilePath, cancellationToken);

                await using var transaction = await _dwContext.Database.BeginTransactionAsync(cancellationToken);

                await DeleteFactSalesAsync(cancellationToken);

                result.TotalFactSales = await InsertFactSalesAsync(
                    stagingData.CustomerList,
                    stagingData.ProductList,
                    stagingData.OrderList,
                    stagingData.OrderDetailList,
                    cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                result.WasSuccessful = true;
                result.Message = "FactSales processed and loaded successfully.";

                _logger.LogInfo("FactSales processing completed.");
            }
            catch (Exception ex)
            {
                result.WasSuccessful = false;
                result.Message = ex.Message;

                _logger.LogFailure("Error while processing FactSales.", ex);
            }

            return result;
        }


        // Lee el archivo de staging y lo deserializa
        private async Task<CsvRawDataBundle> LoadStagingFileAsync(string stagingFilePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(stagingFilePath) || !File.Exists(stagingFilePath))
            {
                throw new FileNotFoundException("Staging file was not found.", stagingFilePath);
            }

            var jsonContent = await File.ReadAllTextAsync(stagingFilePath, cancellationToken);

            var stagingData = JsonSerializer.Deserialize<CsvRawDataBundle>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return stagingData ?? new CsvRawDataBundle();
        }


        // Elimina los registros de FactSales antes de recargar
        private async Task DeleteFactSalesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInfo("Deleting FactSales records...");

            await _dwContext.Database.ExecuteSqlRawAsync("DELETE FROM FactSales", cancellationToken);

            _logger.LogInfo("FactSales cleared.");
        }


        // Elimina todas las dimensiones antes de recargar
        private async Task DeleteDimensionsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInfo("Deleting dimension tables...");

            await _dwContext.Database.ExecuteSqlRawAsync("DELETE FROM DimCustomer", cancellationToken);
            await _dwContext.Database.ExecuteSqlRawAsync("DELETE FROM DimProduct", cancellationToken);
            await _dwContext.Database.ExecuteSqlRawAsync("DELETE FROM DimDate", cancellationToken);
            await _dwContext.Database.ExecuteSqlRawAsync("DELETE FROM DimOrderStatus", cancellationToken);
            await _dwContext.Database.ExecuteSqlRawAsync("DELETE FROM DimLocation", cancellationToken);
            await _dwContext.Database.ExecuteSqlRawAsync("DELETE FROM DimCategory", cancellationToken);

            _logger.LogInfo("Dimension tables cleared.");
        }


        // Carga las categorías únicas de los productos
        private async Task<int> LoadCategoriesAsync(List<ProductRecord> products, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Loading DimCategory...");

            var categoryList = products
                .Where(p => !string.IsNullOrWhiteSpace(p.Category))
                .GroupBy(p => CleanText(p.Category))
                .Select(g => new DimCategory
                {
                    CategoryName = g.Key
                })
                .ToList();

            await _dwContext.Categories.AddRangeAsync(categoryList, cancellationToken);
            await _dwContext.SaveChangesAsync(cancellationToken);

            return categoryList.Count;
        }


        // Carga los productos y los vincula a su categoría
        private async Task<int> LoadProductsAsync(List<ProductRecord> products, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Loading DimProduct...");

            var categoryMap = await _dwContext.Categories
                .AsNoTracking()
                .ToDictionaryAsync(c => c.CategoryName, c => c.CategoryKey, cancellationToken);

            var productList = products.Select(p => new DimProduct
            {
                ProductID_NaturalKey = p.ProductID,
                ProductName = CleanText(p.ProductName),
                CategoryKey = categoryMap.TryGetValue(CleanText(p.Category), out var catKey) ? catKey : null,
                ListPrice = p.Price,
                Stock = p.Stock
            }).ToList();

            await _dwContext.Products.AddRangeAsync(productList, cancellationToken);
            await _dwContext.SaveChangesAsync(cancellationToken);

            return productList.Count;
        }


        // Carga las ubicaciones únicas basadas en los clientes
        private async Task<int> LoadLocationsAsync(List<CustomerRecord> customers, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Loading DimLocation...");

            var locationList = customers
                .Where(c => !string.IsNullOrWhiteSpace(c.Country) && !string.IsNullOrWhiteSpace(c.City))
                .GroupBy(c => new
                {
                    Country = CleanText(c.Country),
                    City = CleanText(c.City)
                })
                .Select(g => new DimLocation
                {
                    Country = g.Key.Country,
                    City = g.Key.City
                })
                .ToList();

            await _dwContext.Locations.AddRangeAsync(locationList, cancellationToken);
            await _dwContext.SaveChangesAsync(cancellationToken);

            return locationList.Count;
        }


        // Carga los clientes y los vincula a su ubicación
        private async Task<int> LoadCustomersAsync(List<CustomerRecord> customers, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Loading DimCustomer...");

            var locationMap = await _dwContext.Locations
                .AsNoTracking()
                .ToDictionaryAsync(
                    l => MakeLocationKey(l.Country, l.City),
                    l => l.LocationKey,
                    cancellationToken);

            var customerList = customers.Select(c => new DimCustomer
            {
                CustomerID_NaturalKey = c.CustomerID,
                FirstName = CleanText(c.FirstName),
                LastName = CleanText(c.LastName),
                Email = string.IsNullOrWhiteSpace(c.Email) ? null : c.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(c.Phone) ? null : c.Phone.Trim(),
                LocationKey = locationMap.TryGetValue(
                    MakeLocationKey(c.Country, c.City),
                    out var locKey) ? locKey : null
            }).ToList();

            await _dwContext.Customers.AddRangeAsync(customerList, cancellationToken);
            await _dwContext.SaveChangesAsync(cancellationToken);

            return customerList.Count;
        }


        // Carga los estados únicos de las órdenes
        private async Task<int> LoadOrderStatusesAsync(List<OrderRecord> orders, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Loading DimOrderStatus...");

            var statusList = orders
                .Where(o => !string.IsNullOrWhiteSpace(o.Status))
                .GroupBy(o => CleanText(o.Status))
                .Select(g => new DimOrderStatus
                {
                    StatusName = g.Key
                })
                .ToList();

            await _dwContext.OrderStatuses.AddRangeAsync(statusList, cancellationToken);
            await _dwContext.SaveChangesAsync(cancellationToken);

            return statusList.Count;
        }


        // Carga las fechas únicas de las órdenes
        private async Task<int> LoadDatesAsync(List<OrderRecord> orders, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Loading DimDate...");

            var dateList = orders
                .Select(o => o.OrderDate.Date)
                .Distinct()
                .OrderBy(d => d)
                .Select(date => new DimDate
                {
                    DateKey = DateHelper.ConvertToDateKey(date),
                    Date = date,
                    Year = (short)date.Year,
                    Quarter = (byte)(((date.Month - 1) / 3) + 1),
                    Month = (byte)date.Month,
                    MonthName = date.ToString("MMMM"),
                    DayOfMonth = (byte)date.Day,
                    DayName = date.ToString("dddd")
                })
                .ToList();

            await _dwContext.Dates.AddRangeAsync(dateList, cancellationToken);
            await _dwContext.SaveChangesAsync(cancellationToken);

            return dateList.Count;
        }


        // Genera los registros de FactSales cruzando los datos de staging con las dimensiones
        private async Task<int> InsertFactSalesAsync(
            List<CustomerRecord> customers,
            List<ProductRecord> products,
            List<OrderRecord> orders,
            List<OrderDetailRecord> orderDetails,
            CancellationToken cancellationToken)
        {
            _logger.LogInfo("Loading FactSales...");

            var orderMap = orders.ToDictionary(o => o.OrderID, o => o);
            var customerCsvMap = customers.ToDictionary(c => c.CustomerID, c => c);
            var productCsvMap = products.ToDictionary(p => p.ProductID, p => p);

            var customerKeyMap = await _dwContext.Customers
                .AsNoTracking()
                .ToDictionaryAsync(c => c.CustomerID_NaturalKey, c => c.CustomerKey, cancellationToken);

            var productKeyMap = await _dwContext.Products
                .AsNoTracking()
                .ToDictionaryAsync(p => p.ProductID_NaturalKey, p => p.ProductKey, cancellationToken);

            var statusKeyMap = await _dwContext.OrderStatuses
                .AsNoTracking()
                .ToDictionaryAsync(s => s.StatusName, s => s.StatusKey, cancellationToken);

            var locationKeyMap = await _dwContext.Locations
                .AsNoTracking()
                .ToDictionaryAsync(
                    l => MakeLocationKey(l.Country, l.City),
                    l => l.LocationKey,
                    cancellationToken);

            var factList = new List<FactSales>();

            foreach (var detail in orderDetails)
            {
                if (!orderMap.TryGetValue(detail.OrderID, out var order))
                {
                    _logger.LogWarning($"Order {detail.OrderID} not found, skipping detail.");
                    continue;
                }

                if (!customerKeyMap.TryGetValue(order.CustomerID, out var customerKey))
                {
                    _logger.LogWarning($"Customer {order.CustomerID} not found in DimCustomer.");
                    continue;
                }

                if (!productKeyMap.TryGetValue(detail.ProductID, out var productKey))
                {
                    _logger.LogWarning($"Product {detail.ProductID} not found in DimProduct.");
                    continue;
                }

                if (!statusKeyMap.TryGetValue(CleanText(order.Status), out var statusKey))
                {
                    _logger.LogWarning($"Status '{order.Status}' not found in DimOrderStatus.");
                    continue;
                }

                var dateKey = DateHelper.ConvertToDateKey(order.OrderDate);

                int? locationKey = null;
                if (customerCsvMap.TryGetValue(order.CustomerID, out var csvCustomer))
                {
                    var locKey = MakeLocationKey(csvCustomer.Country, csvCustomer.City);
                    if (locationKeyMap.TryGetValue(locKey, out var foundLocKey))
                    {
                        locationKey = foundLocKey;
                    }
                }

                decimal effectiveUnitPrice = 0m;

                if (detail.TotalPrice > 0 && detail.Quantity > 0)
                {
                    effectiveUnitPrice = detail.TotalPrice / detail.Quantity;
                }
                else if (detail.TotalPrice > 0)
                {
                    effectiveUnitPrice = detail.TotalPrice;
                }
                else if (productCsvMap.TryGetValue(detail.ProductID, out var csvProduct))
                {
                    effectiveUnitPrice = csvProduct.Price;
                }

                decimal effectiveSalesAmount = detail.TotalPrice > 0
                    ? detail.TotalPrice
                    : detail.Quantity * effectiveUnitPrice;

                factList.Add(new FactSales
                {
                    DateKey = dateKey,
                    CustomerKey = customerKey,
                    ProductKey = productKey,
                    LocationKey = locationKey,
                    StatusKey = statusKey,
                    OrderID = detail.OrderID,
                    Quantity = detail.Quantity,
                    UnitPrice = effectiveUnitPrice,
                    SalesAmount = effectiveSalesAmount
                });
            }

            await _dwContext.Sales.AddRangeAsync(factList, cancellationToken);
            await _dwContext.SaveChangesAsync(cancellationToken);

            return factList.Count;
        }


        // Elimina espacios y retorna vacío si es nulo
        private static string CleanText(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }


        // Construye una llave compuesta para identificar una ubicación
        private static string MakeLocationKey(string? country, string? city)
        {
            return $"{CleanText(country)}|{CleanText(city)}";
        }
    }
}