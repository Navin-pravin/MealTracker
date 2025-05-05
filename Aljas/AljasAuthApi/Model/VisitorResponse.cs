namespace AljasAuthApi.Models
{
    public class VisitorResponse
    {
        public string IdNumber { get; set; }
        public string VisitorName { get; set; }
        public string Email { get; set; }
        public string VisitorCompany { get; set; }
        public string ContactNo { get; set; }
        public string Type { get; set; }
        public string Remarks { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Location { get; set; }
        public string Canteen { get; set; }
        public string DeviceName { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, string> MealType { get; set; } = new Dictionary<string, string>();
    }
}