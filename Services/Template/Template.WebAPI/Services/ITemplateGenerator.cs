using OfficeOpenXml;
using Template.WebAPI.Dictionary;
using Template.WebAPI.DTOs;
using Template.WebAPI.Enums;

namespace Template.WebAPI.Services;

public interface ITemplateGenerator
{
    byte[] GenerateExcelImportTemplate(ImportType type, string lang = "az");
    byte[] GenerateExcelExportTemplate<T>(List<T> data, string lang = "az");
    TemplateDefinitionDto GetTemplateDefinition(ImportType type);
}

public class TemplateGenerator : ITemplateGenerator
{
    private readonly List<TemplateDefinitionDto> _templateDefinitions = new()
    {
        new TemplateDefinitionDto
        {
            Type = ImportType.Customers,
            SheetName = "Customers",
            Columns = new List<TemplateColumnDto>
            {
                new("FirstName", "string", true),
                new("LastName", "string", true),
                new("Email", "string", false),
                new("Phone", "string", false)
            }
        },
        new TemplateDefinitionDto
        {
            Type = ImportType.Products,
            SheetName = "Products",
            Columns = new List<TemplateColumnDto>
            {
                new("ProductName", "string", true),
                new("Description", "string", true),
                new("PurchasePrice", "decimal", true),
                new("SellPrice", "decimal", true),
                new("ImageUrl", "decimal", false),
                new("StockQuantity", "int", false)
            }
        },
        new TemplateDefinitionDto
        {
            Type = ImportType.Warehouses,
            SheetName = "Warehouses",
            Columns = new List<TemplateColumnDto>
            {
                new("Name", "string", true)
            }
        },
        new TemplateDefinitionDto
        {
            Type = ImportType.Shelves,
            SheetName = "Shelves",
            Columns = new List<TemplateColumnDto>
            {
                new("Code", "string", true)
            }
        }
    };

    public TemplateDefinitionDto GetTemplateDefinition(ImportType type)
    {
        var template = _templateDefinitions.FirstOrDefault(t => t.Type == type);
        return template ?? throw new ArgumentException($"Template not found for type: {type}");
    }

    public byte[] GenerateExcelImportTemplate(ImportType type, string lang = "az")
    {
        var template = GetTemplateDefinition(type);

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add(template.SheetName);

        for (int i = 0; i < template.Columns.Count; i++)
        {
            var column = template.Columns[i];

            var translatedName = LocalizationDictionary.ColumnTranslations.TryGetValue(lang, out var dict)
                && dict.TryGetValue(column.ColumnName, out var localized)
                    ? localized : column.ColumnName;

            var dataType = LocalizationDictionary.DataTypeTranslationsAz.TryGetValue(column.DataType.ToLower(), out var dataTypeAz)
                ? dataTypeAz : column.DataType;

            var header = column.IsRequired ? $"{translatedName} *" : translatedName;
            sheet.Cells[1, i + 1].Value = header;
            sheet.Cells[2, i + 1].Value = $"Type: {dataType}";
            sheet.Cells[1, i + 1].Style.Font.Bold = true;
            sheet.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            sheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        sheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    public byte[] GenerateExcelExportTemplate<T>(List<T> data, string lang = "az")
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("ExportedData");
        if (data == null || !data.Any())
        {
            sheet.Cells[1, 1].Value = "No data available";
            return package.GetAsByteArray();
        }
        var properties = typeof(T).GetProperties();
        for (int i = 0; i < properties.Length; i++)
        {
            var property = properties[i];
            var translatedName = LocalizationDictionary.ColumnTranslations.TryGetValue(lang, out var dict)
                && dict.TryGetValue(property.Name, out var localized)
                    ? localized : property.Name;
            sheet.Cells[1, i + 1].Value = translatedName;
            sheet.Cells[1, i + 1].Style.Font.Bold = true;
            sheet.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            sheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }
        for (int rowIndex = 0; rowIndex < data.Count; rowIndex++)
        {
            for (int colIndex = 0; colIndex < properties.Length; colIndex++)
            {
                var value = properties[colIndex].GetValue(data[rowIndex]);
                sheet.Cells[rowIndex + 2, colIndex + 1].Value = value?.ToString() ?? string.Empty;
            }
        }
        sheet.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }
}