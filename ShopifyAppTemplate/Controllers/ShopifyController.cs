using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopifyAppTemplate.Data;
using ShopifyAppTemplate.Extensions;
using ShopifySharp;

using ShopifyAppTemplate.Services;

namespace ShopifyAppTemplate.Controllers;

public class ShopifyController : Controller
{
    private readonly ShopjetsAdminService shopjetsAdmin;
    private readonly ShopjetsDataContext dataContext;
    public ShopifyController(ShopjetsDataContext dataCtx, ShopjetsAdminService sjAdmin)
    {
        dataContext = dataCtx;
        shopjetsAdmin = sjAdmin;
    }

    [HttpPost]
    public async Task<IActionResult> CustomerDataRequest([FromBody] dynamic jsonData)
    {
        try
        {
            var jsonBody = jsonData.ToString();
            Console.WriteLine("============> CustomerDataRequest : Received json body = " + jsonBody);     

            var isAuthentic = AuthorizationService.IsAuthenticWebhook(Request.Headers, jsonBody, AppConstants.ShopifySecretKey);
            if(isAuthentic == true)
            {
                await shopjetsAdmin.ProcessWebhookRequest(jsonData.ToString(), "GDPRCustomerDataRequest");
            }
            else
            {
                return Unauthorized();
            }
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("============> ERROR ShopifyController : CustomerDataRequest");
        }

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CustomerRedact([FromBody] dynamic jsonData)
    {
        try
        {
            var jsonBody = jsonData.ToString();
            Console.WriteLine("============> CustomerRedact : Received json body = " + jsonBody);  

            var isAuthentic = AuthorizationService.IsAuthenticWebhook(Request.Headers, jsonBody, AppConstants.ShopifySecretKey);
            if(isAuthentic == true)
            {
                await shopjetsAdmin.ProcessWebhookRequest(jsonData.ToString(), "GDPRCustomerRedact");
            }
            else
            {
                return Unauthorized();
            }
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("============> ERROR ShopifyController : CustomerRedact");
        }

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ShopRedact([FromBody] dynamic jsonData)
    {
        try
        {
            var jsonBody = jsonData.ToString();
            Console.WriteLine("============> ShopRedact : Received json body = " + jsonBody);
            
            var isAuthentic = AuthorizationService.IsAuthenticWebhook(Request.Headers, jsonBody, AppConstants.ShopifySecretKey);
            if(isAuthentic == true)
            {
                await shopjetsAdmin.ProcessWebhookRequest(jsonData.ToString(), "GDPRShopRedact");
            }
            else
            {
                return Unauthorized();
            }            
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("============> ERROR ShopifyController : ShopRedact");
        }

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> AppUninstalled([FromBody] dynamic jsonData)
    {
        try
        {
            var jsonBody = jsonData.ToString();
            Console.WriteLine("============> AppUninstalled : Received json body = " + jsonBody);

            var isAuthentic = AuthorizationService.IsAuthenticWebhook(Request.Headers, jsonBody, AppConstants.ShopifySecretKey);
            if (isAuthentic == true)
            {
                Console.WriteLine("============> AppUninstalled : JSON request is valid");
                await shopjetsAdmin.ProcessAppUninstall(jsonData.ToString(), "ShopifyAppUninstall");
                return Ok();
            }
            else
            {
                Console.WriteLine("============> AppUninstalled : JSON request is NOT valid");
                return Unauthorized();
            }
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("============> ERROR ShopifyController : AppUninstalled");
        }

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ShopUpdate([FromBody] dynamic jsonData)
    {
        try
        {
            var jsonBody = jsonData.ToString();
            Console.WriteLine("============> ShopUpdate : Received json body = " + jsonBody);

            var isAuthentic = AuthorizationService.IsAuthenticWebhook(Request.Headers, jsonBody, AppConstants.ShopifySecretKey);
            if (isAuthentic == true)
            {
                Console.WriteLine("============> ShopUpdate : JSON request is valid");
                //await shopjetsAdmin.ProcessAppUninstall(jsonData.ToString(), "ShopifyAppUninstall");
                return Ok();
            }
            else
            {
                Console.WriteLine("============> ShopUpdate : JSON request is NOT valid");
                return Unauthorized();
            }
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("============> ERROR ShopifyController : ShopUpdate");
        }

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> ChargeResultCallback([FromQuery] long charge_id)
    {
        Console.WriteLine("============> ShopifyController : ChargeResultCallback : charge id = " + charge_id);
        // Again, grab the customer and make sure they are not already subscribed
        Models.Customer? thisCustomer = null;
        try
        {
            thisCustomer = await dataContext.Customers.FirstAsync(u => u.ShopifyInterimChargeId == charge_id);
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("============> ERROR ShopifyController : ChargeResultCallback : Failed to get customer details. Redirecting to Auth/Index");
            return RedirectToAction("Index", "Auth");
        }

        if (thisCustomer == null)
        {
            Console.WriteLine("============> ShopifyController : ChargeResultCallback : A null object received for customer with interim charge id " + charge_id);
            return RedirectToAction("Index", "Auth");
        }
            

        RecurringCharge charge = null;

        try
        {
            // Get the subscription they're activating
            var service = new RecurringChargeService(thisCustomer.ShopifyShopDomain, thisCustomer.ShopifyAccessToken);
            charge = await service.GetAsync(charge_id);
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("============> ERROR ShopifyController : ChargeResultCallback : Failed to get a valid charge. Redirecting to Auth/Index");
            return RedirectToAction("Index", "Auth");
        }

        if (charge == null)
        {
            Console.WriteLine("============> ShopifyController : ChargeResultCallback : A null object received for charge " + charge_id);
            return RedirectToAction("Index", "Auth");
        }

        // Check the status of the charge
        switch (charge.Status)
        {
            case "pending":
            case "expired":
            case "declined": // User has not accepted or declined the charge. Send them back to the confirmation url                
                Console.WriteLine("============> ShopifyController : ChargeResultCallback : Charge status is pending / expired / declined. Redirect to confirmation URL");
                return Redirect(charge.ConfirmationUrl);

            case "accepted": //Legacy status
            case "active": //User has already accepted this charge and activated it            
                Console.WriteLine("============> ShopifyController : Charge status is active / accepted. Updated to Suscribed status and redirect to Home::Index");
                await HttpContext.SignInAsync(thisCustomer);
                return RedirectToAction("Index", "Home", new { custId = thisCustomer.Id });

            default:
                Console.WriteLine("============> ShopifyController : ChargeResultCallback : Unknown status in callback. Redirect to confirmation URL. Charge status is = " + charge.Status);
                return Redirect(charge.ConfirmationUrl);
        }
    }

}
