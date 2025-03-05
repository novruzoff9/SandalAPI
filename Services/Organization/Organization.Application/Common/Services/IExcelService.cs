using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Organization.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Application.Common.Services;

public interface IExcelService
{
    public Task<List<Warehouse>> ImportWarehouses(IFormFile file, string companyId);
    public Task<List<Product>> ImportProducts(IFormFile file, string companyId);
    public Task<byte[]> ExportToExcel<T>(IEnumerable<T> data) ;
}

public class ExcelService : IExcelService
{
    public Task<byte[]> ExportToExcel<T>(IEnumerable<T> data)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("DataSheet");
            var properties = typeof(T).GetProperties();

            // Add headers
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = properties[i].Name;
            }

            // Add data
            int row = 2;
            foreach (var item in data)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    var value = properties[i].GetValue(item);
                    if (value == null)
                        worksheet.Cells[row, i + 1].Value = string.Empty;
                    else if (value is DateTime dtValue)
                        worksheet.Cells[row, i + 1].Value = dtValue.ToString("yyyy-MM-dd HH:mm:ss");
                    else if (value is bool boolValue)
                        worksheet.Cells[row, i + 1].Value = boolValue ? "Yes" : "No";
                    else
                        worksheet.Cells[row, i + 1].Value = value.ToString();
                }
                row++;
            }

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();

            // Generate byte array
            var fileBytes = package.GetAsByteArray();
            if (fileBytes == null || fileBytes.Length == 0)
                throw new Exception("Failed to generate Excel file.");

            return Task.FromResult(fileBytes);
        }
    }


    public async Task<List<Product>> ImportProducts(IFormFile file, string companyId)
    {
        List<Product> products = new List<Product>();
        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    throw new Exception("No worksheet found in the Excel file.");
                }
                var rowCount = worksheet.Dimension.Rows;
                for (int row = 2; row <= rowCount; row++)
                {
                    var product = new Product
                    {
                        Name = worksheet.Cells[row, 1].Value?.ToString().Trim(),
                        Description = worksheet.Cells[row, 2].Value?.ToString().Trim(),
                        PurchasePrice = decimal.Parse(worksheet.Cells[row, 3].Value?.ToString().Trim() ?? "0"),
                        SellPrice = decimal.Parse(worksheet.Cells[row, 4].Value?.ToString().Trim() ?? "0"),
                        Quantity = int.Parse(worksheet.Cells[row, 5].Value?.ToString().Trim() ?? "0"),
                    };
                    products.Add(product);
                }
            }
        }
        return products;
    }
    public async Task<List<Warehouse>> ImportWarehouses(IFormFile file, string companyId)
    {
        List<Warehouse> warehouses = new List<Warehouse>();
        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    throw new Exception("No worksheet found in the Excel file.");
                }
                var rowCount = worksheet.Dimension.Rows;
                for (int row = 2; row <= rowCount; row++)
                {
                    var warehouse = new Warehouse
                    {
                        Name = worksheet.Cells[row, 1].Value?.ToString().Trim(),
                        City = worksheet.Cells[row, 2].Value?.ToString().Trim(),
                        State = worksheet.Cells[row, 3].Value?.ToString().Trim(),
                        Street = worksheet.Cells[row, 6].Value?.ToString().Trim(),
                        ZipCode = worksheet.Cells[row, 4].Value?.ToString().Trim(),
                        GoogleMaps = worksheet.Cells[row, 5].Value?.ToString().Trim()
                    };
                    warehouses.Add(warehouse);
                }
            }
        }
        return warehouses;
    }
}
