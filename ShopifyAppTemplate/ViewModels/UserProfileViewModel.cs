using ShopifySharp;
using System.Collections.Generic;

namespace ShopifyAppTemplate.ViewModels;

public class UsageMetrics
{
    public UsageMetrics()
    {
        TotalImages = 0;
        StandardFilters = 0;
        AIFilters = 0;
        TopFilters = new List<KeyValuePair<string, int>>();
    }

    public int TotalImages { get; set; }
    public int StandardFilters { get; set; }
    public int AIFilters { get; set; }
    public List<KeyValuePair<string, int>> TopFilters { get; set; }
}

class FilterSorter : IComparer<KeyValuePair<string, int>>
{
    public int Compare(KeyValuePair<string, int> x, KeyValuePair<string, int> y)
    {          
        if (x.Value == 0 || y.Value == 0)
        {
            return 0;
        }
          
        // "CompareTo()" method
        return y.Value.CompareTo(x.Value);          
    }
}
  

public class UserProfileViewModel
{
    public UserProfileViewModel(Shop shop, UsageMetrics metrics)
    {
        Metrics = metrics;
        
        if (shop != null)
        {
            Name = shop.Name;
            Domain = shop.Domain;
            Country = shop.CountryName;            
        }
        else
        {
            Name = string.Empty;
            Domain = string.Empty;
            Country = string.Empty;
        }
    }

    public string Name { get; }
    public string Domain { get; }
    public string Country { get; }
    public UsageMetrics Metrics { get; set; }
}
