namespace Template.WebAPI.Dictionary;

public static class LocalizationDictionary
{
    public static readonly Dictionary<string, Dictionary<string, string>> ColumnTranslations = new()
    {
        ["az"] = new Dictionary<string, string>
        {
            ["FirstName"] = "Müştərinin Adı",
            ["LastName"] = "Müştərinin Soyadı",
            ["Email"] = "Əlaqə Email",
            ["Phone"] = "Telefon Nömrəsi",
            ["ProductName"] = "Məhsul Adı",
            ["Description"] = "Haqqında",
            ["PurchasePrice"] = "Alış Qiyməti",
            ["SellPrice"] = "Satış Qiyməti",
            ["StockQuantity"] = "Stok Miqdarı",
            ["ImageUrl"] = "Şəkil ünvanı",
            ["Code"] = "Kod"
        },
        ["en"] = new Dictionary<string, string>
        {
            // Optional - fallback to original names
        }
    };

    public static readonly Dictionary<string, string> DataTypeTranslationsAz = new()
    {
        ["string"] = "mətn",
        ["int"] = "tam ədəd",
        ["decimal"] = "kəsr ədəd"
    };
}
