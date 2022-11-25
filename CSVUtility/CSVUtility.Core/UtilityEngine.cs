using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CSVUtility.Core.Helpers;
using CSVUtility.Core.Model;
using CSVUtility.Core.Parser;
using ExcelDataReader;

namespace CSVUtility.Core
{
    public class UtilityEngine : IUtilityEngine
    {
        private readonly IOutputHandler Logger;
        public bool SampleProperty { get; set; }
        public string CsvFilePath { get; set; }

        public UtilityEngine(IOutputHandler outputHandler)
        {
            this.Logger = outputHandler;
        }

        public void Action()
        {
            //DisplayHeader();

            Logger.Log($"Reading CSV file from : {CsvFilePath}");

            var orderDetails = ReadOrderDetailFromCsv(CsvFilePath);

            Logger.Log($"Total Order detail(s) count: {orderDetails.Count} from file: {CsvFilePath}");
            Logger.EmptyLine();

            if (orderDetails.Count <= 0)
            {
                Logger.Log($"No Order detail(s) found in file: {CsvFilePath}");
                return;
            }

            Logger.Log($"Processing Business Rule(s)..........");
            Logger.EmptyLine();

            var totalOrders = TotalOrders(orderDetails);
            Logger.Log($"Total Order(s) : {totalOrders}");

            var averagePerOrder = GetAveragePerOrder(orderDetails);
            var generateAvgContent = GenerateAvgContent(averagePerOrder, totalOrders);
            var outputFilePath = GenerateOutputFilePath(CsvFilePath, 0);
            Logger.Log($"Generating CSV file : {outputFilePath} ..........");
            CsvWriteProcess(outputFilePath, generateAvgContent);
            
            
            var orderDetailsLookup = orderDetails.ToLookup(k => k.ProductName, v => v);
            var popularProducts = GetPopularProducts(orderDetailsLookup);
            var generatePopularContent = GeneratePopularContent(popularProducts);
            outputFilePath = GenerateOutputFilePath(CsvFilePath, 1);
            Logger.Log($"Generating CSV file : {outputFilePath} ..........");
            CsvWriteProcess(outputFilePath, generatePopularContent);

            Logger.Log("Tool process completed successfully.");
        }

        public Dictionary<string, string> GetPopularProducts(ILookup<string, OrderDetail> orderDetailsLookup)
        {
            var filterPopularProducts = new Dictionary<string, string>();
            var popularProduct = new Dictionary<string, List<string>>();

            foreach (var details in orderDetailsLookup)
            {
                foreach (var orderDetail in details)
                {
                    if (popularProduct.ContainsKey(details.Key))
                    {
                        var brandOrder = popularProduct[details.Key];
                        brandOrder.Add(orderDetail.Brand);
                    }
                    else
                    {
                        popularProduct.Add(details.Key, new List<string>{orderDetail.Brand});
                    }
                }
            }

            foreach (var product in popularProduct)
            {
                var popularBrand = (from x in product.Value
                    group x by x
                    into g
                    let count = g.Count()
                    orderby count descending
                    select new {Value = g.Key, Count = count}).FirstOrDefault();

                if(popularBrand == null || string.IsNullOrEmpty(popularBrand.Value) || filterPopularProducts.ContainsValue(popularBrand.Value)) continue;

                filterPopularProducts.Add(product.Key, popularBrand.Value);
            }

            return filterPopularProducts;
        }

        private string GenerateAvgContent(Dictionary<string, int> averagePerOrder, int totalOrder)
        {
            var content = new StringBuilder();

            foreach (var productOrder in averagePerOrder)
            {
                var numerator = Convert.ToDecimal(productOrder.Value);
                var denominator = Convert.ToDecimal(totalOrder);
                var avgOrder = numerator / denominator;
                var productAvgString = $"{productOrder.Key},{avgOrder} ";

                Logger.Log($"Product: {productOrder.Key} , Average Order: {avgOrder}");

                content.AppendLine(productAvgString);
            }

            return content.ToString();
        }

        private string GeneratePopularContent(Dictionary<string, string> popularProduct)
        {
            var content = new StringBuilder();

            foreach (var product in popularProduct)
            {
                var productAvgString = $"{product.Key},{product.Value}";

                Logger.Log($"Popular Product: {product.Key} , Brand: {product.Key}");

                content.AppendLine(productAvgString);
            }

            return content.ToString();
        }

        public Dictionary<string, int> GetAveragePerOrder(List<OrderDetail> orderDetailsLookup)
        {
            var productsTotalOrders = new Dictionary<string, int>();

            foreach (var details in orderDetailsLookup)
            {
                if (productsTotalOrders.ContainsKey(details.ProductName))
                {
                    productsTotalOrders[details.ProductName] += details.Quantity;
                }
                else
                {
                    productsTotalOrders.Add(details.ProductName, details.Quantity);
                }
            }

            return productsTotalOrders;
        }

        public int TotalOrders(IEnumerable<OrderDetail> orderDetailsLookup)
        {
            return orderDetailsLookup.Count();
        }

        private string GenerateOutputFilePath(string inputFileName, int index)
        {
            var fileName = Path.GetFileName(inputFileName);
            var directoryName = Path.GetDirectoryName(inputFileName);
            var newFileName = $"{index}_{fileName}";
            return !string.IsNullOrEmpty(directoryName) ? Path.Combine(directoryName, newFileName) : string.Empty;
        }

        private List<OrderDetail> ReadOrderDetailFromCsv(string filePath)
        {
            var orderDetails = new List<OrderDetail>();
            var lineCount = 1;
            try
            {
                using (var r = new StreamReader(filePath, Encoding.UTF8, true))
                {
                    while (r.TryReadCsvRow(out var row))
                    {
                        var reader = row.ToArray();

                        if (reader.Length > 5)
                        {
                            Logger.Log($"Invalid values count at line: {lineCount}", MessageSeverity.Error);
                            continue;
                        }

                        var order = new OrderDetail()
                        {
                            Id = reader[0].Trim(),
                            Area = reader[1].Trim(),
                            ProductName = reader[2].Trim(),
                            Quantity = Convert.ToInt32(reader[3]),
                            Brand = reader[4].Trim()
                        };
                        orderDetails.Add(order);
                        lineCount++;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
                throw;
            }

            return orderDetails;
        }

        private void DisplayHeader()
        {
            Logger.WriteToConsole(
                "********************************* CSV Utility  *********************************",
                MessageSeverity.Info);
            Logger.WriteToConsole("", MessageSeverity.Info);
            Logger.WriteToConsole($"CSV File Path:\t\t\t{CsvFilePath}", MessageSeverity.Info);
            Logger.WriteToConsole("", MessageSeverity.Info);
            Logger.WriteToConsole(
                "*********************************************************************************",
                MessageSeverity.Info);
            Logger.WriteToConsole("STARTING THE TOOL", MessageSeverity.Info);
            Logger.WriteToConsole("", MessageSeverity.Info);
        }

        private void CsvWriteProcess(string csvFilePath, string content)
        {
            using (var writer = new StreamWriter(csvFilePath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false, ShouldQuote = args => false };
                using (var csvWriter = new CsvWriter(writer, config))
                {
                    csvWriter.WriteField(content);
                }
            }
        }
    }
}