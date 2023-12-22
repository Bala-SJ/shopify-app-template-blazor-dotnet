using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ShopifyAppTemplate.Models;

namespace ShopifyAppTemplate.Extensions;

public static class HttpContextExtensions
{
    public static async Task SignInAsync(this HttpContext ctx, Session session)
    {
        List<Session> allSessions = new();
        allSessions.Add(session);

        if (ctx.User.Claims.Count() != 0)
        {
            foreach (var claim in ctx.User.Claims)
            {
                Console.WriteLine("============> HttpExtn : SignInAsync : Appending an existing claim");
                Console.WriteLine("============> HttpExtn : SignInAsync : CLAIM TYPE: " + claim.Type + "; CLAIM VALUE: " + claim.Value);
                if (claim.Value != null)
                {
                    var sessionItem = JsonSerializer.Deserialize<List<Session>>(claim.Value);
                    if (sessionItem != null)
                    {
                        foreach (var valueItem in sessionItem)
                        {
                            if(session.UserId == valueItem.UserId)
                                continue;

                            allSessions.Add(valueItem);
                        }
                    }
                }
            }
        }

        var cookieJson = JsonSerializer.Serialize(allSessions);
        Console.WriteLine("============> HttpExtn : SignInAsync : Cookie JSON = " + cookieJson);

        var claims = new List<Claim>
            {
                new Claim("UserSession", cookieJson, ClaimValueTypes.String)
            };

        var authScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        var identity = new ClaimsIdentity(claims, authScheme);
        var principal = new ClaimsPrincipal(identity);
        await ctx.SignInAsync(principal);
    }
    
    public static async Task SignInAsync(this HttpContext ctx, ShopifyAppTemplate.Models.Customer userAccount)
    {
        await SignInAsync(ctx, new Session(userAccount));
    }
    public static List<Session> GetAllUserSessions(this ClaimsPrincipal userPrincipal)
    {
        if (!userPrincipal.Identity.IsAuthenticated)
        {
            throw new Exception("User is not authenticated, cannot get user session.");
        }

        List<Session> allSessions = new();

        if (null != userPrincipal)
        {
            foreach (var claim in userPrincipal.Claims)
            {
                Console.WriteLine("============> HttpExtn : CLAIM TYPE: " + claim.Type + "; CLAIM VALUE: " + claim.Value);
                if (claim.Value != null && claim.Type != null && claim.Type == "UserSession")
                {
                    allSessions = JsonSerializer.Deserialize<List<Session>>(claim.Value);
                }
            }
        }

        return allSessions;
    }
}

