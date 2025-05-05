namespace AljasAuthApi.Models
{
    public class SubContractorResponse
    {
        public string ContractorId { get; set; }
        public string ContractorName { get; set; }
        public string CompanyName { get; set; }
        public string ProjectName { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string Nationality { get; set; }
        public string VehicleName { get; set; }
        public string VehicleId { get; set; }
        public string ContractorImage { get; set; } 
        public string Location { get; set; }
        public string Canteen { get; set; }
        public string DeviceName { get; set; }
        public DateTime Date { get; set; }
       public Dictionary<string, string> MealType { get; set; } = new Dictionary<string, string>();
    }
    
}