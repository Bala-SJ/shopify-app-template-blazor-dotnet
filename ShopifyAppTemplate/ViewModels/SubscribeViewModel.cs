using ShopifyAppTemplate.Models;

namespace ShopifyAppTemplate.ViewModels;
public class SubscribeViewModel
{
    public long CustId { get; set; }

    public SubscribeViewModel(long _custId)
    {
        CustId = _custId;
    }
}

