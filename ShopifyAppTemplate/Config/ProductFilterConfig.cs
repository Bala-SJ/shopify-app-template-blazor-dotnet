using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using ShopifyAppTemplate.Config;
namespace ShopifyAppTemplate.Config;
public class ProductFilterConfig
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    public ProductStatusConfig Status { get; set; } = ProductStatusConfig.All;
    public string? Title { get; set; }
    public string? Tag { get; set; }
    public string? Vendor { get; set; }
    public string? Collection { get; set; }

    public void Reset()
    {
        Id = string.Empty;
        Status = ProductStatusConfig.All;
    }
}