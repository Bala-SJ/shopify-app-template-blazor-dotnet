using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using ShopifySharp;

using ShopifyAppTemplate.Models;
using ShopifyAppTemplate.Data;
using ShopifyAppTemplate.Extensions;
using ShopifyAppTemplate.ViewModels;
using ShopifyAppTemplate.Services;

namespace ShopifyAppTemplate.Controllers;

public class AuthController : Controller
{
    private readonly ShopjetsDataContext dataContext;
    private readonly ShopjetsAdminService shopjetsAdmin;
    public AuthController(ShopjetsDataContext _dataContext, ShopjetsAdminService _shopjetsAdmin)
    {
        dataContext = _dataContext;
        shopjetsAdmin = _shopjetsAdmin;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new LoginViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Login(string shop, string hmac, string host, string timestamp)
    {
        Console.WriteLine("============> Auth : Login : Shop = " + shop);
        Console.WriteLine("============> Auth : Login : HMAC = " + hmac);
        Console.WriteLine("============> Auth : Login : Host = " + host);
        Console.WriteLine("============> Auth : Login : Timestamp = " + timestamp);

        try
        {
            if (false == await AuthorizationService.IsValidShopDomainAsync(shop))
            {
                return RedirectToAction("Index", "Auth");
            }
        }
        catch (Exception)
        {
            return RedirectToAction("Index", "Auth");
        }

        Models.Customer? customerDetails = null;

        // try
        // {
        //     var allSessions = HttpContext.User.GetAllUserSessions();
        //     foreach (var session in allSessions)
        //     {
        //         Console.WriteLine("============> Auth : Login : GetAllUserSessions = " + session.UserId);
        //         var cDetail = await shopjetsAdmin.GetCustomerDetailsById(session.UserId);
        //         if (cDetail != null && cDetail.ShopifyShopDomain == shop)
        //         {
        //             customerDetails = cDetail;
        //         }
        //     }
        // }
        // catch (Exception)
        // {
        //     return OauthAuthorizeRedirect(shop);
        // }

        if (customerDetails == null)
        {
            Console.WriteLine("============> Auth : Login : New user workflow");
            return OauthAuthorizeRedirect(shop);
        }
        else
        {
            switch (customerDetails.ShopJetsStatus)
            {
                case (int)SJCustomerStatus.Unset:
                case (int)SJCustomerStatus.Uninstalled:
                    Console.WriteLine("============> Auth : Login : Unset/Uninstalled status : New user workflow");
                    return OauthAuthorizeRedirect(shop);
                    break;

                default:
                    Console.WriteLine("============> Auth : Login : App state is installed. Redirect to app for custId = " + customerDetails.Id);
                    return RedirectToAction("Index", "Home", new { custId = customerDetails.Id });
                    break;
            }
        }
    }

    [HttpGet]
    public async Task<IActionResult> ResultCallback(string shop, string code, string state)
    {
        Console.WriteLine("============> Auth : ResultCallback : Shop = " + shop);
        Console.WriteLine("============> Auth : ResultCallback : Code = " + code);
        Console.WriteLine("============> Auth : ResultCallback : State = " + state);

        // Check to make sure the state token has not already been used
        var stateToken = await dataContext.LoginStates.FirstOrDefaultAsync(t => t.Token == state);

        if (stateToken == null)
        {
            // This token has already been used. The user must go through the OAuth process again
            Console.WriteLine("============> Auth : ResultCallback : Token already used. Reinitiate OAuth process");
            return RedirectToAction("Index", "Auth");
        }

        // Delete the token so it can't be used again
        dataContext.LoginStates.Remove(stateToken);
        await dataContext.SaveChangesAsync();

        // Exchange the code param for a permanent Shopify access token
        var accessToken = await AuthorizationService.Authorize(code, shop, AppConstants.ShopifyPublicKey, AppConstants.ShopifySecretKey);

        // Use the access token to get the user's shop info
        var shopService = new ShopService(shop, accessToken);
        var shopData = await shopService.GetAsync();

        Console.WriteLine("============> Auth : ResultCallback : Received new token = " + accessToken);

        // Check to see if the user's account already exists and should be updated, or if it needs to be created
        var user = await dataContext.Customers.FirstOrDefaultAsync(u => u.ShopifyShopDomain == shop);

        if (user == null)
        {
            // Create the user's account
            Console.WriteLine("============> Auth : ResultCallback : Creating new user");
            user = new ShopifyAppTemplate.Models.Customer
            {
                ShopifyAccessToken = accessToken,
                ShopifyShopDomain = shop,
                ShopifyShopId = shopData.Id.Value,
                ShopJetsStatus = (int)SJCustomerStatus.Installed,
            };

            dataContext.Add(user);
        }
        else
        {
            // Update the user's account
            Console.WriteLine("============> Auth : ResultCallback : Update existing user");
            user.ShopifyAccessToken = accessToken;
            user.ShopifyShopDomain = shop;
            user.ShopifyShopId = shopData.Id.Value;
            if( user.ShopJetsStatus == (int)SJCustomerStatus.Uninstalled )
                user.ShopJetsStatus = (int)SJCustomerStatus.Installed;
        }

        await shopjetsAdmin.RegisterWebhook(shop, accessToken, "app/uninstalled", AppConstants.AppUninstalledWebhookUrl);
        await shopjetsAdmin.RegisterWebhook(shop, accessToken, "shop/update", AppConstants.ShopUpdateWebhookUrl);

        await dataContext.SaveChangesAsync();

        // Sign the user in
        await HttpContext.SignInAsync(user);

        // User has installed the app, send them to the app for further processing
        Console.WriteLine("============> Auth : ResultCallback : App installed. Redirect to app for custId = " + user.Id);
        return RedirectToAction("Index", "Home", new { custId = user.Id });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }

    /////////////////////////////////////////
    //////////PRIVATE HELPERS////////////////
    /////////////////////////////////////////

    private IActionResult OauthAuthorizeRedirect(string shop)
    {
        var requiredPermissions = new[] { "read_product_listings", "read_products", "write_products", "read_files", "write_files" };
        var oauthState = dataContext.LoginStates.Add(new OAuthState
        {
            DateCreated = DateTimeOffset.Now,
            Token = Guid.NewGuid().ToString()
        });

        dataContext.SaveChanges();

        var oauthUrl = AuthorizationService.BuildAuthorizationUrl(
            requiredPermissions,
            shop,
            AppConstants.ShopifyPublicKey,
            AppConstants.AuthResultCallbackUrl,
            oauthState.Entity.Token);
        Console.WriteLine("============> Login : OauthAuthorizeRedirect : " + oauthUrl.ToString());

        return Redirect(oauthUrl.ToString());
    }

}