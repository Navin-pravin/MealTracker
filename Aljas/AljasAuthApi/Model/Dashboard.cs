using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Dashboard
{
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string DashboardName { get; set; }
    public string LocationId { get; set; }
    public string LocationName { get; set; }
    public string CanteenId { get; set; }
    public string CanteenName { get; set; }
    
    public List<string> MealTypes { get; set; } = new();
}

public class DashboardPreviewRequest
{
     public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string DashboardName { get; set; }
    public string LocationId { get; set; }
    public string LocationName { get; set; }
    public string CanteenId { get; set; }
    public string CanteenName { get; set; }
}
public class DashboardMealCountSummary
{
    public string Dashboardid{get;set;}
    public string DashboardName { get; set; }
    public string LocationId { get; set; }
    public string CanteenId { get; set; }
    public MealCountSummary MealTypeCounts { get; set; }
      public int TotalToday { get; set; }
    public int TotalThisWeek { get; set; }
    public int TotalThisMonth { get; set; }
}

public class MealCountSummary
{
    public Dictionary<string, int> Today { get; set; }
    public Dictionary<string, int> ThisWeek { get; set; }
    public Dictionary<string, int> ThisMonth { get; set; }
}

