namespace ShopifyAppTemplate.Models;

public class OAuthState
{
    public int Id { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public string Token { get; set; } = string.Empty;
}