using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Organization.Domain.Entities;
using Shared.Services;
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

public class ExcelService : BaseExcelService, IExcelService
{
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
                    var warehouse = new Warehouse(
                        worksheet.Cells[row, 1].Value!.ToString().Trim(),
                        worksheet.Cells[row, 2].Value?.ToString().Trim(),
                        worksheet.Cells[row, 3].Value?.ToString().Trim(),
                        worksheet.Cells[row, 6].Value?.ToString().Trim(),
                        worksheet.Cells[row, 4].Value?.ToString().Trim(),
                        worksheet.Cells[row, 5].Value?.ToString().Trim(),
                        companyId
                    );
                    warehouses.Add(warehouse);
                }
            }
        }
        return warehouses;
    }
}
