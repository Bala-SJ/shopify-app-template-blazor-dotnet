using System.Text.Json;
using ShopifySharp;
using ShopifyAppTemplate.Config;
using ShopifyAppTemplate.State;

namespace ShopifyAppTemplate.Services;

public class ShopifyClient
{
    private readonly static int pageSize = 15;
    private ProductImageService? productImageService;
    private ProductService? productService;
    private List<ProductImage> productImages;
    private List<Product> products;
    public ShopifyClient()
    {
        productImages = new();
        products = new();
    }

    public async Task InitializeClient(string storeUrl, string storeToken)
    {
        if (productImageService == null)
        {
            productImageService = new ProductImageService(storeUrl, storeToken);
        }

        if (productService == null)
        {
            productService = new ProductService(storeUrl, storeToken);
        }

        Reset();

        var shopifyProducts = await productService.ListAsync();
        foreach (var prodItem in shopifyProducts.Items)
        {
            products.Add(prodItem);
            foreach (var imgItem in prodItem.Images)
            {
                productImages.Add(imgItem);
            }
        }
    }

    public int GetMaxPages()
    {
        return productImages.Count() / pageSize;
    }

    public void Reset()
    {
        if (productImages != null)
        {
            productImages.Clear();
        }
        if (products != null)
        {
            products.Clear();
        }
    }

    public IEnumerable<ProductImage> GetPage(int page)
    {
        return productImages.Skip(page * pageSize).Take(pageSize);
    }

    public IEnumerable<ProductImage> GetFilteredProducts(AppState appState)
    {
        IEnumerable<Product> filteredProducts = products;

        if (appState.CurrentCustomer == null || appState.PFConfig == null || productImages == null || products == null)
            return productImages;

        if (appState.PFConfig.Status > ProductStatusConfig.All)
        {
            filteredProducts = filteredProducts.Where(p => p.Status == ProductStatusConfigNames.GetName(appState.PFConfig.Status));
        }

        if (appState.PFConfig.Title != null && appState.PFConfig.Title != string.Empty)
        {
            filteredProducts = filteredProducts.Where(p => p.Title.Contains(appState.PFConfig.Title, StringComparison.OrdinalIgnoreCase));
        }

        if (appState.PFConfig.Tag != null && appState.PFConfig.Tag != string.Empty)
        {
            filteredProducts = filteredProducts.Where(p =>
            {
                var tokens = p.Tags.Split(',');
                foreach (var item in tokens)
                {
                    if (appState.PFConfig.Tag.Equals(item.Trim(), StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            });
        }

        productImages.Clear();
        foreach (var prodItem in filteredProducts)
        {
            foreach (var imgItem in prodItem.Images)
            {
                productImages.Add(imgItem);
            }
        }

        return productImages;
    }

    public async Task RegisterWebhook(string storeUrl, string storeToken, string topic, string webhookUrl)
    {
        try
        {
            var webhookService = new WebhookService(storeUrl, storeToken);
            if(webhookService == null)
                return;

            bool isAlreadyRegistered = false;
            var allWebhooks = await webhookService.ListAsync();
            foreach (var item in allWebhooks.Items)
            {
                if (item.Topic == topic)
                {
                    isAlreadyRegistered = true;

                }
            }

            if (isAlreadyRegistered == false)
            {
                var hook = new Webhook()
                {
                    Address = webhookUrl,
                    Format = "json",
                    Topic = topic,
                };

                hook = await webhookService.CreateAsync(hook);
                Console.WriteLine("============> RegisterWebhook : Created webhook for topic =  " + topic + " | webhook URL = " + webhookUrl + " | " + JsonSerializer.Serialize(hook));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("============> RegisterWebhook : ERROR");
        }
    }

    public async Task UnregisterAppUninstallWebhooks(string storeUrl, string storeToken)
    {
        var webhookService = new WebhookService(storeUrl, storeToken);

        if (webhookService == null)
            return;

        var allWebhooks = await webhookService.ListAsync();
        foreach (var item in allWebhooks.Items)
        {
            if (item.Topic == "app/uninstalled" && item.Id != null)
            {
                await webhookService.DeleteAsync(item.Id.Value);
            }
        }
    }

    public async Task<RecurringCharge> GetCurrentCharge(Models.Customer currentCustomer)
    {
        RecurringCharge currentCharge = null;
        
        if(currentCustomer.ShopifyChargeId != null)
        {
            var recurringChargeService = new RecurringChargeService(currentCustomer.ShopifyShopDomain, currentCustomer.ShopifyAccessToken);
            currentCharge = await recurringChargeService.GetAsync(currentCustomer.ShopifyChargeId.Value);
        }
        
        return currentCharge;
    }

    public async Task DeleteRecurringCharge(Models.Customer currentCustomer)
    {
        if(currentCustomer.ShopifyChargeId != null)
        {
            var recurringChargeService = new RecurringChargeService(currentCustomer.ShopifyShopDomain, currentCustomer.ShopifyAccessToken);
            await recurringChargeService.DeleteAsync(currentCustomer.ShopifyChargeId.Value);
        }
    }
}