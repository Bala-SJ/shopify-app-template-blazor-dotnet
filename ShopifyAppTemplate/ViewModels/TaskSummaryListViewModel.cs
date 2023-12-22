namespace ShopifyAppTemplate.ViewModels;

public class TaskSummaryViewItem
{
    public static IDictionary<string, string> SStyle = new Dictionary<string, string>(){
        { "Submitted", "badge rounded-pill mb-2  bg-info"},
        { "Processing", "badge rounded-pill mb-2 bg-primary"},
        { "Completed", "badge rounded-pill mb-2  bg-success"},
        { "Reverted", "badge rounded-pill mb-2  bg-success"},
        { "Error", "badge rounded-pill mb-2 bg-danger"}
    };

    public List<string> TaskDescriptions { get; set; } = new();
    public int NumImages { get; set; } = 0;
    public int GroupId { get; set; } = 0;
    public string GroupDocId { get; set; } = string.Empty;
    public string OprName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string HumanisedTimestamp { get; set; } = string.Empty;
    public string StatusStyle { get; set; } = string.Empty;
    public string ProgressValueNow { get; set; } = "0";
    public string ProgressValueStyle { get; set; } = "width : 0%";
}