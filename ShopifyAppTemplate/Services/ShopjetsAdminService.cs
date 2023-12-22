using System.Text.Json;
using System.Text.Json.Nodes;

using ShopifySharp;

using ShopifyAppTemplate.Data;
using ShopifyAppTemplate.Models;
using ShopifyAppTemplate.ViewModels;
using ShopifyAppTemplate.Config;
using ShopifyAppTemplate.State;

namespace ShopifyAppTemplate.Services;

public class ShopjetsAdminService
{
    private ShopjetsDataContext dataContext;
    private ShopifyClient shopifyClient;
    private GraphQLClient graphQLClient;
    public ShopjetsAdminService(ShopjetsDataContext _dataContext)
    {
        dataContext = _dataContext;
        shopifyClient = new ShopifyClient();        
        graphQLClient = new GraphQLClient();
    }

    public IEnumerable<ProductImage> GetFilteredProducts(AppState appState)
    {
        return shopifyClient.GetFilteredProducts(appState);
    }

    public async Task InitializeClient(string storeUrl, string storeToken)
    {
        await shopifyClient.InitializeClient(storeUrl, storeToken);
    }

    public int GetMaxPages()
    {
        return shopifyClient.GetMaxPages();
    }

    public void ResetProductImages()
    {
        shopifyClient.Reset();
    }

    public IEnumerable<ProductImage> GetProductImagePage(int page)
    {
        return shopifyClient.GetPage(page);
    }

    public async Task RegisterWebhook(string storeUrl, string storeToken, string topic, string webhookUrl)
    {
        await shopifyClient.RegisterWebhook(storeUrl, storeToken, topic, webhookUrl);
        return;
    }

    public async Task<RecurringCharge> GetCurrentCharge(Models.Customer currentCustomer)
    {
        var currentCharge = await shopifyClient.GetCurrentCharge(currentCustomer);
        return currentCharge;
    }

    public async Task DeleteSubscription(Models.Customer currentCustomer)
    {
        await shopifyClient.DeleteRecurringCharge(currentCustomer);
    }
    


    public async Task ProcessWebhookRequest(string jsonBody, string reqType)
    {
        try
        {
            var payloadObj = new WebhookPayload();
            payloadObj.Payload = jsonBody;
            payloadObj.Type = reqType;
        }
        catch (System.Exception e)
        {
            Console.WriteLine("######## ERROR ShopjetsAdmin : ProcessWebhookRequest", e.Message);
        }

    }

    public async Task ProcessAppUninstall(string jsonBody, string reqType)
    {
        try
        {            
            var payloadObj = new WebhookPayload();
            payloadObj.Payload = jsonBody;
            payloadObj.Type = reqType;

            var jsonDOM = JsonNode.Parse(jsonBody);
            if (jsonDOM == null)
                return;

            Console.WriteLine("============> ShopjetsAdminService : ProcessAppUninstall : JSON DOM is valid");
            
            var shopId = jsonDOM["id"]?.GetValue<long>();
            if(shopId == null)
                return;                  

            Console.WriteLine("============> ShopjetsAdminService : ProcessAppUninstall : Shop Id from JSON DOM  = " + shopId);
            
            //TODO : Bala : The following code flow will not work
            //We get this callback because the app was uninstalled. At this point
            //Shopify will deactivate the store token. So no further Shopify API 
            //calls are possible. Is it really necessary to remove the registered webhooks ? 

            //Delete the app/uninstall webhook registered on this shop
            // string storeUrl = string.Empty;
            // string storeToken = string.Empty;
            // var statusVal = postgreSQLClient.GetShopifyDetailsFromShopId(shopId.Value, ref storeUrl, ref storeToken);

            // if(statusVal == true)
            // {            
            //     await shopifyClient.UnregisterAppUninstallWebhooks(storeUrl, storeToken);
            // }            
        }
        catch (System.Exception e)
        {
            Console.WriteLine("######## ERROR ShopjetsAdmin : ProcessAppUninstall", e.Message);
        }

    }
}