using System.Text.Json;
using System.Net.Http.Headers;

namespace ShopifyAppTemplate.Services;

public class UPIReqImage
{
    public string id { get; set; } = string.Empty;
    public string src { get; set; } = string.Empty;
};
public class UPIReqVariables
{
    public string productId { get; set; } = string.Empty;
    public UPIReqImage image { get; set; } = new();
};

public class UpdateProductImageRequest
{
    public string query { get; set; } = string.Empty;
    
    public UPIReqVariables variables { get; set; } = new();
    
    public UpdateProductImageRequest(long productId, long productImageId, string imageUrl)
    {
        query = "mutation productImageUpdate($productId: ID!, $image: ImageInput!){ productImageUpdate(productId: $productId, image: $image) { image { id src } userErrors { field message } } }";
        variables.productId = "gid://shopify/Product/" + productId.ToString();
        variables.image.id = "gid://shopify/ProductImage/" + productImageId.ToString();
        variables.image.src = imageUrl;
    }
};

public class GraphQLClient
{
    private HttpClient httpClient;
    private readonly string shopifyGraphQLUrl = "/admin/api/2022-07/graphql.json";
    public GraphQLClient()
    {
        httpClient = new HttpClient(new HttpClientHandler() { MaxConnectionsPerServer = 40 });
    }
    public async Task<string> UpdateProductImage(string shopName, string accessToken, UpdateProductImageRequest upiRequest)
    {
        string processResponse = string.Empty;
        
        try
        {
            var shopGQLUrl = "https://" + shopName + shopifyGraphQLUrl;
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), shopGQLUrl))
            {
                request.Headers.TryAddWithoutValidation("X-Shopify-Access-Token", accessToken);

                var requestIdsStr = JsonSerializer.Serialize(upiRequest);
                request.Content = new StringContent(requestIdsStr);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                Console.WriteLine(requestIdsStr);

                var response = await httpClient.SendAsync(request);
                processResponse = await response.Content.ReadAsStringAsync();
                System.Console.WriteLine(processResponse);
            }
        }
        catch
        {
            Console.WriteLine("Unknown exception in UpdateProductImage");
        }

        return processResponse;
    }
}