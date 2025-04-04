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

        public async Task<bool> GenerateCouponsAsync(string couponCode, int count, DateTime startDate, DateTime endDate, string description)
        {
            var coupons = new List<Coupon>();
            for (int i = 1; i <= count; i++)
            {
                coupons.Add(new Coupon
                {
                    CouponCode = couponCode,
                    GeneratedCode = $"{couponCode}{i:D3}",
                    StartDate = startDate,
                    EndDate = endDate,
                    Description = description,
                    Status = true
                });
            }
            await _coupons.InsertManyAsync(coupons);
            return true;
        }

        public async Task<List<Coupon>> GetCouponsAsync() =>
            await _coupons.Find(_ => true).ToListAsync();

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
