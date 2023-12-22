using ShopifyAppTemplate.Data;
using ShopifyAppTemplate.Services;
using ShopifyAppTemplate.State;
using ShopifyAppTemplate.Controllers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Radzen;
using Radzen.Blazor;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure HSTS
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(90);
    options.IncludeSubDomains = true;
    options.Preload = true;
});

// Add services to the container.
builder.Services.AddServerSideBlazor();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ShopjetsAdminService>();
builder.Services.AddScoped<AppState>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "ShopJetsAuthCookie";
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.LoginPath = "/";
        options.Validate();
    });

var ShopJetsShopifyOrigins = "_ShopJetsShopifyOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(ShopJetsShopifyOrigins,
        policy =>
        {
            policy.WithOrigins("http://shopify.com", "http://myshopify.com").AllowAnyHeader().AllowAnyMethod();
        });
});

//Adding Radzen services
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Add other security headers
app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
app.UseRouting();
app.UseCors(ShopJetsShopifyOrigins);
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        "OnlyAction",
        "{action}",
        new { controller = "Home", action = "Index" });

    endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapBlazorHub();
});

app.Run();