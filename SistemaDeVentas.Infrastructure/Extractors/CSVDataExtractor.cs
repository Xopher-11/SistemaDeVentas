using Microsoft.Extensions.Options;
using SistemaDeVentas.Application.Interfaces;
using SistemaDeVentas.Application.Models;
using SistemaDeVentas.Application.SourceModels.CSV;
using SistemaDeVentas.Infrastructure.Settings;
using System.Diagnostics;

namespace SistemaDeVentas.Infrastructure.Extractors
{
    public class CsvDataExtractor : ISalesDataExtractor
    {
        private readonly ICsvFileReader<CustomerRecord> _customerReader;
        private readonly ICsvFileReader<ProductRecord> _productReader;
        private readonly ICsvFileReader<OrderRecord> _orderReader;
        private readonly ICsvFileReader<OrderDetailRecord> _orderDetailReader;
        private readonly IStagingAreaService _stagingService;
        private readonly IProcessLogger _logger;
        private readonly CSVSettings _csvSettings;

        public string DataSourceName => "CSV Files";

        public CsvDataExtractor(
            ICsvFileReader<CustomerRecord> customerReader,
            ICsvFileReader<ProductRecord> productReader,
            ICsvFileReader<OrderRecord> orderReader,
            ICsvFileReader<OrderDetailRecord> orderDetailReader,
            IStagingAreaService stagingService,
            IProcessLogger logger,
            IOptions<CSVSettings> csvOptions)
        {
            _customerReader = customerReader;
            _productReader = productReader;
            _orderReader = orderReader;
            _orderDetailReader = orderDetailReader;
            _stagingService = stagingService;
            _logger = logger;
            _csvSettings = csvOptions.Value;
        }

        public async Task<SourceExtractionInfo> ExtractDataAsync(CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.Now;
            var timer = Stopwatch.StartNew();

            try
            {
                _logger.LogInfo("Starting CSV extraction...");

                var customersPath = Path.Combine(_csvSettings.BasePath, _csvSettings.CustomersFileName);
                var productsPath = Path.Combine(_csvSettings.BasePath, _csvSettings.ProductsFileName);
                var ordersPath = Path.Combine(_csvSettings.BasePath, _csvSettings.OrdersFileName);
                var orderDetailsPath = Path.Combine(_csvSettings.BasePath, _csvSettings.OrderDetailsFileName);

                _logger.LogInfo($"Customers path: {customersPath}");
                _logger.LogInfo($"Products path: {productsPath}");
                _logger.LogInfo($"Orders path: {ordersPath}");
                _logger.LogInfo($"OrderDetails path: {orderDetailsPath}");

                if (!File.Exists(customersPath))
                    throw new FileNotFoundException("Customers CSV not found.", customersPath);

                if (!File.Exists(productsPath))
                    throw new FileNotFoundException("Products CSV not found.", productsPath);

                if (!File.Exists(ordersPath))
                    throw new FileNotFoundException("Orders CSV not found.", ordersPath);

                if (!File.Exists(orderDetailsPath))
                    throw new FileNotFoundException("OrderDetails CSV not found.", orderDetailsPath);

                var customers = await _customerReader.ReadFileAsync(customersPath, cancellationToken);
                var products = await _productReader.ReadFileAsync(productsPath, cancellationToken);
                var orders = await _orderReader.ReadFileAsync(ordersPath, cancellationToken);
                var orderDetails = await _orderDetailReader.ReadFileAsync(orderDetailsPath, cancellationToken);

                var productPriceMap = products.ToDictionary(p => p.ProductID, p => p.Price);

                foreach (var detail in orderDetails)
                {
                    if (detail.TotalPrice <= 0 &&
                        productPriceMap.TryGetValue(detail.ProductID, out var fallbackPrice))
                    {
                        detail.TotalPrice = fallbackPrice * detail.Quantity;
                    }
                }

                var extractedData = new CsvRawDataBundle
                {
                    CustomerList = customers,
                    ProductList = products,
                    OrderList = orders,
                    OrderDetailList = orderDetails
                };

                var staging = await _stagingService.SaveToStagingAsync(
                    extractedData,
                    "csv_extraction.json",
                    cancellationToken);

                _logger.LogInfo($"CSV staging file created: {staging.FilePath}");

                timer.Stop();

                var totalRecords = customers.Count + products.Count + orders.Count + orderDetails.Count;

                return new SourceExtractionInfo
                {
                    SourceName = DataSourceName,
                    WasSuccessful = true,
                    RecordsExtracted = totalRecords,
                    Message = "CSV extraction completed successfully.",
                    StagingFilePath = staging.FilePath,
                    StartedAt = startTime,
                    EndedAt = DateTime.Now,
                    DurationInMilliseconds = timer.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                timer.Stop();
                _logger.LogFailure("CSV extraction failed.", ex);

                return new SourceExtractionInfo
                {
                    SourceName = DataSourceName,
                    WasSuccessful = false,
                    RecordsExtracted = 0,
                    Message = ex.ToString(),
                    StartedAt = startTime,
                    EndedAt = DateTime.Now,
                    DurationInMilliseconds = timer.ElapsedMilliseconds
                };
            }
        }
    }
}