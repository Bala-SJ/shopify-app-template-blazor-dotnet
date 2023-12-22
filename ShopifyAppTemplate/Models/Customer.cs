namespace ShopifyAppTemplate.Models;

public enum SJCustomerStatus:int { Unset=0, Uninstalled=1, Unsubscribed=2, Installed=3, Subscribed=4, QuotaExceededStandard=5, QuotaExceededAI=6, QuotaExceededAll=7 }
public enum SJAppPlans:int { Unset=0, Basic=2, Standard=3, Premium=4 }

public class Customer
{
    public int Id { get; set; }
    public long ShopifyShopId { get; set; }
    public string ShopifyShopDomain { get; set; } = null!;
    public string ShopifyAccessToken { get; set; } = null!;
    public long? ShopifyChargeId { get; set; }
    public long? ShopifyInterimChargeId { get; set; }
    public int ShopJetsPlanId { get; set; } = (int)SJAppPlans.Unset;
    public int ShopJetsInterimPlanId { get; set; } = (int)SJAppPlans.Unset;
    public int ShopJetsStatus { get; set; } = (int)SJCustomerStatus.Unset;

    public bool IsInstalled()
    {
        return ( ShopJetsStatus >= (int)SJCustomerStatus.Installed );
    }
    public bool IsSubscribed()
    {
        return ( ShopJetsStatus >= (int)SJCustomerStatus.Subscribed );
    }
    public bool IsStandardQuotaExceeded()
    {
        return ( ShopJetsStatus == (int)SJCustomerStatus.QuotaExceededStandard || ShopJetsStatus == (int)SJCustomerStatus.QuotaExceededAll);
    }
    public bool IsAIQuotaExceeded()
    {
        return false;
    }
    public bool IsUsingBasicPlan()
    {
        return ( ShopJetsPlanId == (int)SJAppPlans.Basic );
    }
}
