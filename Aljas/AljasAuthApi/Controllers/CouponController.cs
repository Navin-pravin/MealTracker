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
 [HttpPost("generate-coupons")]
        public async Task<IActionResult> GenerateCoupons([FromBody] Coupon request, [FromQuery] int count)
        {
            if (count <= 0)
                return BadRequest(new { message = "Count must be greater than 0" });

            bool result = await _couponService.GenerateCouponsAsync(
                request.CouponCode,     // Just used as a placeholder, actual code is generated
                count,
                request.StartDate,
                request.EndDate,
                request.Description
            );

            if (!result)
                return BadRequest(new { message = "Failed to generate coupons" });

            return Ok(new { message = "Coupons generated successfully" });
        }


        [HttpGet]
        public async Task<ActionResult<List<Coupon>>> GetCoupons()
        {
            return Ok(await _couponService.GetCouponsAsync());
        }
[HttpGet("active-coupons")]
public async Task<IActionResult> GetActiveCoupons()
{
    var activeCoupons = await _couponService.GetActiveCouponsAsync();
    return Ok(activeCoupons);
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
