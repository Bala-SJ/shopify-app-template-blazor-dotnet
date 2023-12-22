using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShopifyAppTemplate.Data;

namespace ShopifyAppTemplate.Controllers;

public class HomeController : Controller
{
    private readonly ShopjetsDataContext dataContext;

    public HomeController(ShopjetsDataContext _dataContext)
    {
        dataContext = _dataContext;
    }

    public async Task<IActionResult> Index(int custId)
    {
        Console.WriteLine("============> Home : Index : CustId = " + custId);
        Models.Customer? thisCustomer = null;

        try
        {
            thisCustomer = await dataContext.Customers.FirstAsync(u => u.Id == custId);
            
            if (thisCustomer == null)
                return RedirectToAction("Index", "Auth");

            ViewBag.CurrentCustomer = thisCustomer;

            return View();
        }
        catch (System.Exception)
        {
            Console.WriteLine("============> ERROR : Home : Index : Redirecting to Auth/Index. CustId = " + custId);
            return RedirectToAction("Index", "Auth");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}

