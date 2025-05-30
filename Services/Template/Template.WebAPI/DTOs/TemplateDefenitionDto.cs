using Template.WebAPI.Enums;

namespace Template.WebAPI.DTOs;

public class TemplateDefinitionDto
{
    public ImportType Type { get; set; }
    public string SheetName { get; set; }
    public List<TemplateColumnDto> Columns { get; set; }
}

public record TemplateColumnDto(string ColumnName, string DataType, bool IsRequired);
