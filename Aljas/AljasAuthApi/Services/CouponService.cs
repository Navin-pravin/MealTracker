// Services/CouponService.cs
using MongoDB.Driver;
using ProjectHierarchyApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;


namespace ProjectHierarchyApi.Services
{
    public class CouponService
    {
        private readonly IMongoCollection<Coupon> _coupons;

        public CouponService(IMongoDatabase database)
        {
            _coupons = database.GetCollection<Coupon>("Coupons");
        }
public async Task<bool> GenerateCouponsAsync(string baseCode, int count, DateTime startDate, DateTime endDate, string description)
{
    var utcNow = DateTime.UtcNow;
    var datePrefix = $"{utcNow:yyyyMMdd}"; // "20250415"
    var prefix = $"CV{datePrefix}";

    // Fetch existing coupon codes with this prefix
    var existingCoupons = await _coupons.Find(c =>
        c.CouponCode.StartsWith(prefix)).ToListAsync();

    // Extract the max serial number already used for today
    int maxSerial = 0;
    foreach (var coupon in existingCoupons)
    {
        string code = coupon.CouponCode;
        if (code.Length == prefix.Length + 3 && int.TryParse(code.Substring(prefix.Length), out int num))
        {
            if (num > maxSerial)
                maxSerial = num;
        }
    }

    var newCoupons = new List<Coupon>();
    int serial = maxSerial + 1;

    while (newCoupons.Count < count)
    {
        string serialPart = serial.ToString("D3"); // 3-digit padding
        string generatedCode = $"{prefix}{serialPart}";

        newCoupons.Add(new Coupon
        {
            CouponCode = generatedCode,
            StartDate = startDate,
            EndDate = endDate,
            Description = description,
            Status = true,
            Createdat = utcNow,
            Modifiedat = utcNow,
            Createdby = "System",
            Modifiedby = "System",
            Redeemstatus = false
        });

        serial++;
        if (serial > 9999) break; // Safety limit
    }

    if (newCoupons.Count > 0)
        await _coupons.InsertManyAsync(newCoupons);

    return true;
}


        public async Task<List<Coupon>> GetCouponsAsync() =>
            await _coupons.Find(_ => true).ToListAsync();

    public async Task<List<Coupon>> GetActiveCouponsAsync()
{
    var filter = Builders<Coupon>.Filter.And(
        Builders<Coupon>.Filter.Eq(c => c.Status, true),
        Builders<Coupon>.Filter.Eq(c => c.Redeemstatus, false)
    );

    var activeCoupons = await _coupons.Find(filter).ToListAsync();
    return activeCoupons;
}
public async Task<string> GetNextCouponCodeAsync()
{
    string prefix = $"CV{DateTime.UtcNow:yyyyMMdd}";

    // Find all coupons that start with today's prefix
    var filter = Builders<Coupon>.Filter.Regex(c => c.CouponCode, new BsonRegularExpression($"^{prefix}"));
    var coupons = await _coupons.Find(filter).ToListAsync();

    // Extract sequence numbers
    int maxNumber = 0;
    foreach (var coupon in coupons)
    {
        if (coupon.CouponCode.Length >= prefix.Length + 3 &&
            int.TryParse(coupon.CouponCode.Substring(prefix.Length), out int number))
        {
            if (number > maxNumber)
                maxNumber = number;
        }
    }

    int nextNumber = maxNumber + 1;
    string nextCouponCode = $"{prefix}{nextNumber:D3}";
    return nextCouponCode;
}


        public async Task<Coupon?> GetCouponByIdAsync(string id) =>
            await _coupons.Find(c => c.Id == id).FirstOrDefaultAsync();

        public async Task<bool> UpdateCouponStatusAsync(string id, bool status)
        {
            var update = Builders<Coupon>.Update.Set(c => c.Status, status);
            var result = await _coupons.UpdateOneAsync(c => c.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteCouponAsync(string id)
        {
            var result = await _coupons.DeleteOneAsync(c => c.Id == id);
            return result.DeletedCount > 0;
        }

       public async Task<List<string>> AssignMultipleCouponsAsync(List<CouponAssignRequest> requests)
{
    List<string> failedCouponIds = new();

    foreach (var request in requests)
    {
        var coupon = await _coupons.Find(c => c.Id == request.CouponId).FirstOrDefaultAsync();
        if (coupon == null)
        {
            failedCouponIds.Add(request.CouponId);
            continue;
        }

        bool isModified =
            coupon.Assignedto != request.AssignedTo ||
            coupon.Redeemstatus != request.RedeemStatus ||
            coupon.Redeemedcanteen != request.RedeemedCanteen;

        var update = Builders<Coupon>.Update
            .Set(c => c.Assignedto, request.AssignedTo)
            .Set(c => c.Assignedat, request.AssignedAt)
            .Set(c => c.Redeemstatus, request.RedeemStatus)
            .Set(c => c.Redeemedcanteen, request.RedeemedCanteen)
            .Set(c => c.Createdby, request.AssignedBy)
            .Set(c => c.AssignedEmployee, request.AssignedEmployee);

        if (isModified)
        {
            update = update
                .Set(c => c.Modifiedby, request.AssignedBy)
                .Set(c => c.Modifiedat, DateTime.UtcNow);
        }

        var result = await _coupons.UpdateOneAsync(
            Builders<Coupon>.Filter.Eq(c => c.Id, request.CouponId),
            update
        );

        if (result.ModifiedCount == 0)
        {
            failedCouponIds.Add(request.CouponId);
        }
    }

    return failedCouponIds;
}

    }
}
