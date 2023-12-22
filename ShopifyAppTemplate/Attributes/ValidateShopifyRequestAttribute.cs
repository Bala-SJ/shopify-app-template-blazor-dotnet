using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using ShopifySharp;

namespace ShopifyAppTemplate.Attributes;

public class ValidateShopifyRequestAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var isAuthentic = AuthorizationService.IsAuthenticRequest(context.HttpContext.Request.Query, AppConstants.ShopifySecretKey);

        if (isAuthentic)
        {
            await next();
        }
        else
        {
            context.Result = new ForbidResult();
        }
    }
}
