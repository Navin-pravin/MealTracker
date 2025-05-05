namespace AljasAuthApi.Models
{
    public class EmployeeResponse
    {
        public required string IDNumber { get; set; }
        public required string Name { get; set; }
        public  string Designation { get; set; }
        public required string Department { get; set; }
        public required string Role { get; set; }
        public required string Company { get; set; }
        public required string Type { get; set; }
        public required string StartDate { get; set; }
        public required string EndDate { get; set; }
        public required string PhoneNumber { get; set; }
        public required string CardBadgeNumber { get; set; }
        public required string Location { get; set; }
        public required string Canteen { get; set; }
        public required string DeviceName { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, string> MealType { get; set; } = new Dictionary<string, string>();
        
    }
    public class MealRecord
    {
        public string MealType { get; set; }  // "Breakfast", "Lunch", etc.
        public string Time { get; set; }      // Format: "HH:mm"
        public DateTime FullDateTime { get; set; }  // Original timestamp
    }
    
}