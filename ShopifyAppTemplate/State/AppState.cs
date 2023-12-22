using ShopifySharp;
using ShopifyAppTemplate.Models;
using ShopifyAppTemplate.Config;
using System.Collections.Generic;

namespace ShopifyAppTemplate.State;

public class AppState
{
    public ShopifyAppTemplate.Models.Customer? CurrentCustomer { get; set; }
    public ProductFilterConfig PFConfig { get; set; }
    public IEnumerable<ProductImage> ProductImages { get; set; }
    public ProductImage? EditorProductImage { get; set; }

    public event Action OnCVResultChange;

    public AppState()
    {
        PFConfig = new();
        ProductImages = new List<ProductImage>();
    }

    public void NotifyCVResultChanged() => OnCVResultChange?.Invoke();

    public void ClearEditorState()
    {
        EditorProductImage = null;
    }
}