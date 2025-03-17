using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public abstract class BaseExcelService
    {
        public Task<byte[]> ExportToExcel<T>(IEnumerable<T> data)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
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
    }
}
