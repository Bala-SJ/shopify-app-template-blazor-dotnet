using ShopifyAppTemplate.Models;
using ShopifySharp;

namespace ShopifyAppTemplate.ViewModels;

public class CurrentSubscriptionViewModel
{
    public CurrentSubscriptionViewModel(RecurringCharge charge, long custId)
    {
        ChargeName = charge.Name;
        DateCreated = charge.BillingOn.Value;
        TrialEndsOn = charge.TrialEndsOn.Value;
        CustId = custId;
    }

    public string ChargeName { get; }
    public DateTimeOffset DateCreated { get; }
    public DateTimeOffset? TrialEndsOn { get; }
    public bool IsTrialing => TrialEndsOn.HasValue;
    public long? CustId { get; }
}
