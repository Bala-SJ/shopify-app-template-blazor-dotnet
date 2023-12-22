namespace ShopifyAppTemplate.Config;

public enum ProductStatusConfig { Unset, All, Active, Draft, Archived }

public class ProductStatusConfigNames
{
    private static IDictionary<ProductStatusConfig, string> Table = new Dictionary<ProductStatusConfig, string>(){
        {ProductStatusConfig.Unset, "unset"},
        {ProductStatusConfig.All, "all"},
        {ProductStatusConfig.Active, "active"},
        {ProductStatusConfig.Draft, "draft"},
        {ProductStatusConfig.Archived, "archived"}
    };

    public static string GetName(ProductStatusConfig psConfig)
    {
        return Table[psConfig];
    }
    public static string GetUXName(ProductStatusConfig psConfig)
    {
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Table[psConfig]);
    }
};
