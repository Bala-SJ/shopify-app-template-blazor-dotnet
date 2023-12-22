using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ShopifyAppTemplate.Config;

public class WebhookPayload
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; }  = string.Empty;
    public string Payload { get; set; } = string.Empty;          
    public void Reset()
    {
        Id = string.Empty;
        Type = string.Empty;
        Payload = string.Empty;        
    }  
}