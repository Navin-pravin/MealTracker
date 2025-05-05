using AljasAuthApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectHierarchyApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AljasAuthApi.Services
{
    public class ConsolidatedReportService
    {
        private readonly IMongoCollection<RawData> _rawDataCollection;
        private readonly IMongoCollection<Employee> _employeeCollection;
        private readonly IMongoCollection<SubContractor> _subContractorCollection;
        private readonly IMongoCollection<Visitor> _visitorCollection;
        private readonly IMongoCollection<Device> _deviceCollection;
        private readonly IMongoCollection<ConsolidatedReportModel> _consolidatedReportCollection;

        public ConsolidatedReportService(IMongoDatabase database)
        {
            _rawDataCollection = database.GetCollection<RawData>("RawData");
            _employeeCollection = database.GetCollection<Employee>("Employees");
            _subContractorCollection = database.GetCollection<SubContractor>("SubContractors");
            _visitorCollection = database.GetCollection<Visitor>("Visitors");
            _deviceCollection = database.GetCollection<Device>("Devices");
            _consolidatedReportCollection = database.GetCollection<ConsolidatedReportModel>("ConsolidatedReports");
        }

        public async Task<ConsolidatedReportWithSummary> GenerateConsolidatedReport(ConsolidatedReportRequest request)
        {
            var filterBuilder = Builders<RawData>.Filter;
            var filters = new List<FilterDefinition<RawData>>
            {
                filterBuilder.Gte(x => x.CreatedDateUtc, request.StartDate),
                filterBuilder.Lte(x => x.CreatedDateUtc, request.EndDate)
            };

            if (!string.IsNullOrWhiteSpace(request.PersonType))
            {
                filters.Add(filterBuilder.Eq(x => x.Type, request.PersonType));
            }

            string canteenNameDisplay = "All";
            var allDeviceIds = new List<string>();

            var validCanteenIds = request.CanteenIds?
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToList();

            if (validCanteenIds != null && validCanteenIds.Any())
            {
                var canteenNames = new List<string>();

                foreach (var canteenId in validCanteenIds)
                {
                    if (ObjectId.TryParse(canteenId, out ObjectId canteenObjectId))
                    {
                        var devices = await _deviceCollection
                            .Find(d => d.CanteenId == canteenObjectId.ToString())
                            .ToListAsync();

                        if (devices.Any())
                        {
                            allDeviceIds.AddRange(devices.Select(d => d.UniqueId));
                            canteenNames.AddRange(devices.Select(d => d.CanteenName));
                        }
                    }
                }

                if (allDeviceIds.Any())
                {
                    filters.Add(filterBuilder.In(x => x.DeviceTrigger, allDeviceIds));
                    canteenNameDisplay = string.Join(", ", canteenNames.Distinct());
                }
                else
                {
                    return new ConsolidatedReportWithSummary
                    {
                        IndividualReports = new List<ConsolidatedReportResponse>(),
                        Summary = new ReportSummary
                        {
                            CanteenName = canteenNameDisplay,
                            StartDate = request.StartDate,
                            EndDate = request.EndDate
                        }
                    };
                }
            }

            var validMealTypes = request.MealTypes?
                .Where(mt => !string.IsNullOrWhiteSpace(mt))
                .ToList();

            if (validMealTypes != null && validMealTypes.Any())
            {
                filters.Add(filterBuilder.In(x => x.MealType, validMealTypes));
            }

            var finalFilter = filterBuilder.And(filters);
            var rawDataList = await _rawDataCollection.Find(finalFilter).ToListAsync();

            if (!rawDataList.Any())
                return new ConsolidatedReportWithSummary
                {
                    IndividualReports = new List<ConsolidatedReportResponse>(),
                    Summary = new ReportSummary
                    {
                        CanteenName = canteenNameDisplay,
                        StartDate = request.StartDate,
                        EndDate = request.EndDate
                    }
                };

            var distinctMealTypes = rawDataList
                .Select(r => r.MealType)
                .Where(m => !string.IsNullOrEmpty(m))
                .Distinct()
                .ToList();

            var groupedData = rawDataList
                .GroupBy(r => new { PersonId = r.SensorAttributes?.PersonId, Date = r.CreatedDateUtc.Date })
                .ToList();

            var personGroups = rawDataList
                .GroupBy(r => r.SensorAttributes?.PersonId)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .ToList();

            var reportResponses = new List<ConsolidatedReportResponse>();
            var totalMealCounts = new Dictionary<string, int>();

            foreach (var personGroup in personGroups)
            {
                var personId = personGroup.Key;
                var firstRecord = personGroup.First();
                var personType = firstRecord.Type;

                var response = new ConsolidatedReportResponse
                {
                    PersonId = personId,
                    Type = personType,
                    UniqueDaysCount = groupedData.Count(g => g.Key.PersonId == personId),
                    MealCounts = new Dictionary<string, int>()
                };

                foreach (var meal in distinctMealTypes)
                {
                    var count = personGroup.Count(r => r.MealType == meal);
                    response.MealCounts[meal] = count;

                    if (totalMealCounts.ContainsKey(meal))
                    {
                        totalMealCounts[meal] += count;
                    }
                    else
                    {
                        totalMealCounts[meal] = count;
                    }
                }

                switch (personType)
                {
                    case "Employee":
                        var employee = await _employeeCollection.Find(e => e.IDNumber == personId).FirstOrDefaultAsync();
                        if (employee != null)
                        {
                            response.Name = $"{employee.Firstname} {employee.Lastname}".Trim();
                            response.Department = employee.Dept;
                            response.Company = employee.Company;
                        }
                        break;

                    case "Sub-Contractor":
                        var contractor = await _subContractorCollection.Find(c => c.ContractorId == personId).FirstOrDefaultAsync();
                        if (contractor != null)
                        {
                            response.Name = contractor.ContractorName;
                            response.Company = contractor.CompanyName;
                            response.Project = contractor.ProjectName;
                        }
                        break;

                    case "Visitor":
                        var visitor = await _visitorCollection.Find(v => v.IdNumber == personId).FirstOrDefaultAsync();
                        if (visitor != null)
                        {
                            response.Name = visitor.VisitorName;
                            response.Company = visitor.VisitorCompany;
                        }
                        break;
                }

                var device = await _deviceCollection.Find(d => d.UniqueId == firstRecord.DeviceTrigger).FirstOrDefaultAsync();
                if (device != null)
                {
                    response.Location = device.LocationName;
                    response.Canteen = device.CanteenName;
                }

                reportResponses.Add(response);
            }

            return new ConsolidatedReportWithSummary
            {
                IndividualReports = reportResponses,
                Summary = new ReportSummary
                {
                    TotalUniqueIndividuals = reportResponses.Count,
                    TotalUniqueDays = reportResponses.Sum(r => r.UniqueDaysCount),
                    TotalMealsServed = reportResponses.Sum(r => r.MealCounts.Values.Sum()),
                    MealTypeCounts = totalMealCounts,
                    CanteenName = canteenNameDisplay,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                }
            };
        }
    }
}