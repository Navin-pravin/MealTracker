using AljasAuthApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectHierarchyApi.Models;

namespace AljasAuthApi.Services
{
    public class ReportService
    {
        private readonly IMongoCollection<RawData> _rawDataCollection;
        private readonly IMongoCollection<Employee> _employeeCollection;
        private readonly IMongoCollection<SubContractor> _subContractorCollection;
        private readonly IMongoCollection<Visitor> _visitorCollection;
        private readonly IMongoCollection<Device> _deviceCollection;
        private readonly IMongoCollection<ReportModel> _reportCollection;
        private readonly IMongoCollection<ConsolidatedReportModel> _consolidatedReportCollection;

        public ReportService(IMongoDatabase database)
        {
            _rawDataCollection = database.GetCollection<RawData>("RawData");
            _employeeCollection = database.GetCollection<Employee>("Employees");
            _subContractorCollection = database.GetCollection<SubContractor>("SubContractors");
            _visitorCollection = database.GetCollection<Visitor>("Visitors");
            _deviceCollection = database.GetCollection<Device>("Devices");
            _reportCollection = database.GetCollection<ReportModel>("Reports");
            _consolidatedReportCollection = database.GetCollection<ConsolidatedReportModel>("ConsolidatedReports");
        }

        public async Task<List<object>> GenerateReport(ReportRequest request)
        {
            var filterBuilder = Builders<RawData>.Filter;
            var filters = new List<FilterDefinition<RawData>>
            {
                filterBuilder.Gte(x => x.CreatedDateUtc, request.StartDate),
                filterBuilder.Lte(x => x.CreatedDateUtc, request.EndDate)
            };

            if (!string.IsNullOrWhiteSpace(request.Type) && request.Type != "All")
                filters.Add(filterBuilder.Eq(x => x.Type, request.Type));

            if (request.MealTypes != null && request.MealTypes.Any(mt => !string.IsNullOrWhiteSpace(mt)))
            {
                var mealTypesFiltered = request.MealTypes.Where(mt => !string.IsNullOrWhiteSpace(mt)).ToList();
                if (mealTypesFiltered.Any())
                    filters.Add(filterBuilder.In(x => x.MealType, mealTypesFiltered));
            }

            List<string> deviceIds = new();
            List<string> canteenNames = new();

            if (request.CanteenIds != null && request.CanteenIds.Any(id => !string.IsNullOrWhiteSpace(id)))
            {
                foreach (var canteenId in request.CanteenIds)
                {
                    if (ObjectId.TryParse(canteenId, out ObjectId objectId))
                    {
                        var devices = await _deviceCollection.Find(d => d.CanteenId == objectId.ToString()).ToListAsync();
                        deviceIds.AddRange(devices.Select(d => d.UniqueId));
                        canteenNames.AddRange(devices.Select(d => d.CanteenName ?? "Unknown"));
                    }
                }

                if (deviceIds.Any())
                    filters.Add(filterBuilder.In(x => x.DeviceTrigger, deviceIds));
                else
                    return new List<object>();
            }

            var finalFilter = filterBuilder.And(filters);
            var rawDataList = await _rawDataCollection.Find(finalFilter).ToListAsync();

            var allMealTypes = rawDataList
                .Select(r => r.MealType)
                .Where(mt => !string.IsNullOrWhiteSpace(mt))
                .Distinct()
                .OrderBy(mt => mt)
                .ToList();

            var reportList = new List<object>();
            var groupedData = rawDataList
                .GroupBy(r => new
                {
                    Type = r.Type,
                    PersonId = r.SensorAttributes?.PersonId,
                    Date = r.CreatedDateUtc.Date
                })
                .ToList();

            foreach (var group in groupedData)
            {
                var type = group.Key.Type;
                var personId = group.Key.PersonId;
                var date = group.First().CreatedDateUtc;

                if (string.IsNullOrEmpty(personId)) continue;

                var personMealTypes = group
                    .Select(g => g.MealType)
                    .Where(mt => !string.IsNullOrEmpty(mt))
                    .Distinct()
                    .ToList();

                var mealTimeDict = group
                    .GroupBy(g => g.MealType)
                    .Where(grp => !string.IsNullOrEmpty(grp.Key))
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.OrderBy(x => x.CreatedDateUtc).First().CreatedDateUtc.ToString("hh:mm tt")
                    );

                var mealTypeDict = allMealTypes.ToDictionary(
                    mt => mt,
                    mt => personMealTypes.Contains(mt) ? "Yes" : ""
                );

                var firstRawData = group.First();
                var device = await _deviceCollection.Find(x => x.UniqueId == firstRawData.DeviceTrigger).FirstOrDefaultAsync();

                switch (type)
                {
                    case "Employee":
                        var employee = await _employeeCollection.Find(x => x.IDNumber == personId).FirstOrDefaultAsync();
                        if (employee != null)
                        {
                            reportList.Add(new
                            {
                                employee.IDNumber,
                                Name = $"{employee.Firstname} {employee.Lastname}".Trim(),
                                employee.Dept,
                                employee.Role,
                                employee.Company,
                                Type = "Employee",
                                employee.StartDate,
                                employee.EndDate,
                                PhoneNumber = employee.Phone_no,
                                employee.CardBadgeNumber,
                                Location = device?.LocationName,
                                Canteen = device?.CanteenName,
                                DeviceName = device?.DeviceName,
                                Date = date.ToString("yyyy-MM-dd"),
                                MealType = mealTypeDict,
                                MealTime = mealTimeDict
                            });
                        }
                        break;

                    case "Sub-Contractor":
                        var sub = await _subContractorCollection.Find(x => x.ContractorId == personId).FirstOrDefaultAsync();
                        if (sub != null)
                        {
                            reportList.Add(new
                            {
                                sub.ContractorId,
                                ContractorName = sub.ContractorName,
                                sub.CompanyName,
                                sub.ProjectName,
                                Type = "Contractor",
                                sub.Address,
                                sub.PhoneNo,
                                sub.Nationality,
                                sub.VehicleName,
                                sub.VehicleId,
                                ContractorImage = sub.ImageUrl,
                                Location = device?.LocationName,
                                Canteen = device?.CanteenName,
                                DeviceName = device?.DeviceName,
                                Date = date.ToString("yyyy-MM-dd "),
                                MealType = mealTypeDict,
                                MealTime = mealTimeDict
                            });
                        }
                        break;

                    case "Visitor":
                        var visitor = await _visitorCollection.Find(x => x.IdNumber == personId).FirstOrDefaultAsync();
                        if (visitor != null)
                        {
                            reportList.Add(new
                            {
                                visitor.IdNumber,
                                VisitorName = visitor.VisitorName,
                                visitor.Email,
                                visitor.VisitorCompany,
                                Type = "Visitor",
                                visitor.ContactNo,
                                visitor.Remarks,
                                visitor.StartDate,
                                visitor.EndDate,
                                Location = device?.LocationName,
                                Canteen = device?.CanteenName,
                                DeviceName = device?.DeviceName,
                                Date = date.ToString("yyyy-MM-dd"),
                                MealType = mealTypeDict,
                                MealTime = mealTimeDict
                            });
                        }
                        break;
                }
            }

            var existingReportFilter = Builders<ReportModel>.Filter.Eq(r => r.Type, request.Type ?? "All");

var updateDefinition = Builders<ReportModel>.Update
        .Set(r => r.StartDate, request.StartDate)
        .Set(r => r.EndDate, request.EndDate)
        .Set(r => r.Count, reportList.Count) 
        .Set(r => r.CanteenName, null) 
        .Set(r => r.MealType, null);

    var updateOptions = new UpdateOptions { IsUpsert = true };

    var result = await _reportCollection.UpdateOneAsync(
        existingReportFilter, 
        updateDefinition,      
        updateOptions        
    );

            return reportList;
        }
    }
}