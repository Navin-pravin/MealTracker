using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AljasAuthApi.Models
{
    public class ConsolidatedReportRequest : ReportRequest
    {
        public string? PersonType { get; set; } // Employee, Sub-Contractor, Visitor
        public List<string>? CanteenIds { get; set; } // Updated: List of Canteen Ids
        public List<string>? MealTypes { get; set; } 
    }

    
  public class ConsolidatedReportResponse
{
    public string PersonId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string? Department { get; set; }
    public string? Company { get; set; }
    public string? Project { get; set; }
    public string? Location { get; set; }
    public string? Canteen { get; set; }
    public Dictionary<string, int> MealCounts { get; set; } = new();

    // TotalMeals should not be set directly, it's calculated dynamically
    
    public int UniqueDaysCount { get; set; }

    
}
    public class ConsolidatedReportModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string ReportName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string PersonType { get; set; } // "Employee", "Sub-Contractor", "Visitor", or "All"
    public DateTime CreatedOn { get; set; }
    public int Count { get; set; } // Total people count in the report
    public List<string> CanteenIds { get; set; }
    public string CanteenName { get; set; } // Name of the canteen or "All"

    [BsonIgnoreIfNull]
    public string? Type { get; set; } // Optional field

    [BsonIgnoreIfNull]
    public DateTime? GeneratedOn { get; set; } // Optional field
    public List<ConsolidatedReportResponse> ReportData { get; set; }
    
}
public class ConsolidatedReportWithSummary
{
    public List<ConsolidatedReportResponse> IndividualReports { get; set; }
    public ReportSummary Summary { get; set; }
}

public class ReportSummary
{
    public int TotalUniqueIndividuals { get; set; }
    public int TotalUniqueDays { get; set; }
    public int TotalMealsServed { get; set; }
    public Dictionary<string, int> MealTypeCounts { get; set; }
    public string CanteenName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
} 

}