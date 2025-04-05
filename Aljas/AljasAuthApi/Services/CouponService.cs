// Services/CouponService.cs
using MongoDB.Driver;
using ProjectHierarchyApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    var datePrefix = $"{utcNow:yyyy-MM-dd}"; // formats as "2025-04-04"

    // Fetch existing coupon codes with this UTC date prefix
    var existingCoupons = await _coupons.Find(c =>
        c.CouponCode.StartsWith(datePrefix)).ToListAsync();

    var existingCodes = new HashSet<string>(existingCoupons.Select(c => c.CouponCode));
    var newCoupons = new List<Coupon>();
    int serial = 1;

    while (newCoupons.Count < count)
    {
        string serialPart = serial.ToString("D3");
        string generatedCode = $"{datePrefix}-{serialPart}";

        if (!existingCodes.Contains(generatedCode))
        {
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
                Redeemstatus = true
            });
        }

        serial++;
        if (serial > 9999) break; // safety check
    }

    if (newCoupons.Count > 0)
        await _coupons.InsertManyAsync(newCoupons);

    return true;
}


        public async Task<List<Coupon>> GetCouponsAsync() =>
            await _coupons.Find(_ => true).ToListAsync();

     public async Task<List<Coupon>> GetActiveCouponsAsync()
{
    var filter = Builders<Coupon>.Filter.Eq(c => c.Status, true);
    var activeCoupons = await _coupons.Find(filter).ToListAsync();
    return activeCoupons;
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
    }
}
