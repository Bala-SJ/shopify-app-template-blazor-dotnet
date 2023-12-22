namespace ShopifyAppTemplate.Models;

public class Session
{
    public Session(ShopifyAppTemplate.Models.Customer user)
    {
        UserId = user.Id;
        IsSubscribed = user.IsSubscribed();
    }
    public Session()
    {
    }
    public int UserId { get; set; }
    public bool IsSubscribed { get; set; }
}