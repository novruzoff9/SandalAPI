using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Common.Services;

public interface IExcelService
{
    public Task<List<Domain.Entities.Order>> ImportOrders(IFormFile file, string companyId);
    public Task<byte[]> ExportToExcel<T>(IEnumerable<T> data);
}

public class ExcelService : BaseExcelService, IExcelService
{
    public async Task<List<Domain.Entities.Order>> ImportOrders(IFormFile file, string companyId)
    {
        List<Domain.Entities.Order> orders = new List<Domain.Entities.Order>();
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
                    var order = new Domain.Entities.Order();
                    orders.Add(order);
                }
            }
        }
        return orders;
    }
}
