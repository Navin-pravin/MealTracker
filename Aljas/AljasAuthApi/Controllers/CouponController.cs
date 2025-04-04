// Controllers/CouponController.cs
using Microsoft.AspNetCore.Mvc;
using ProjectHierarchyApi.Models;
using ProjectHierarchyApi.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectHierarchyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly CouponService _couponService;

        public CouponController(CouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateCoupons([FromBody] Coupon request, [FromQuery] int count)
        {
            if (await _couponService.GenerateCouponsAsync(request.CouponCode, count, request.StartDate, request.EndDate, request.Description))
            {
                return Ok("Coupons generated successfully.");
            }
            return BadRequest("Failed to generate coupons.");
        }

        [HttpGet]
        public async Task<ActionResult<List<Coupon>>> GetCoupons()
        {
            return Ok(await _couponService.GetCouponsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Coupon>> GetCouponById(string id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null) return NotFound();
            return Ok(coupon);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateCouponStatus(string id, [FromQuery] bool status)
        {
            if (await _couponService.UpdateCouponStatusAsync(id, status))
            {
                return Ok("Coupon status updated successfully.");
            }
            return BadRequest("Failed to update coupon status.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(string id)
        {
            if (await _couponService.DeleteCouponAsync(id))
            {
                return Ok("Coupon deleted successfully.");
            }
            return BadRequest("Failed to delete coupon.");
        }
    }
}
