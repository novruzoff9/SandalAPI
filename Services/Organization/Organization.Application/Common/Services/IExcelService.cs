using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organization.Application.Common.Services;

public interface IExcelService
{
    public Task<List<Product>> ImportProducts(IFormFile file, string companyId);
    public Task<string> ExportToExcel<T>(IEnumerable<T> data);
}

public class ExcelService : IExcelService
{
    public Task<string> ExportToExcel<T>(IEnumerable<T> data)
    {
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");
            var properties = typeof(T).GetProperties();

            // Add header
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
                    worksheet.Cells[row, i + 1].Value = properties[i].GetValue(item)?.ToString();
                }
                row++;
            }

            // Save the file
            var filePath = Path.Combine(Path.GetTempPath(), $"exported_data.xlsx");
            package.SaveAs(new FileInfo(filePath));
            return Task.FromResult(filePath);
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
}
