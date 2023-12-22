using Microsoft.AspNetCore.Mvc;

namespace ShopifyAppTemplate.ViewModels;
public class LoginViewModel
{
    [BindProperty(Name = "shop")]
    public string ShopDomain { get; set; } = String.Empty;
    public string Error { get; set; } = string.Empty;
    public bool ShowError => !string.IsNullOrWhiteSpace(Error);
}

