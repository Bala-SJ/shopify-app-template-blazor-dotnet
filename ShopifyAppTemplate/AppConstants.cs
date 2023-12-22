public class AppConstants
{
    ///////////////////////////////////////////////////////////    
    //TODO MAJOR : Using null suppression is ugly. What is a better way ?
    public static string ShopifySecretKey { get; } = Environment.GetEnvironmentVariable("SHOPIFY_APP_SECRET_KEY")!;
    public static string ShopifyPublicKey { get; } = Environment.GetEnvironmentVariable("SHOPIFY_APP_PUBLIC_KEY")!;
    ///////////////////////////////////////////////////////////
    public static string ProdBaseImgAppUrl { get; } = "https://app-name.public-domain.com";
    public static string DevBaseImgAppUrl { get; } = "https://dev-app-name.public-domain.com";
    public static string LocalBaseImgAppUrl { get; } = "https://ae0a-14-194-93-115.ngrok-free.app";
    public static string BaseImgAppUrl { get; } = ProdBaseImgAppUrl;
    ///////////////////////////////////////////////////////////
    public static string AuthResultCallbackUrl { get; } = BaseImgAppUrl + "/Auth/ResultCallback";
    public static string AppUninstalledWebhookUrl { get; } = BaseImgAppUrl + "/Shopify/AppUninstalled";  
    public static string ShopUpdateWebhookUrl { get; } = BaseImgAppUrl + "/Shopify/ShopUpdate";  
    public static string SubscriptionChargeResultUrl { get; } = BaseImgAppUrl + "/Shopify/ChargeResultCallback";        
}
