using Microsoft.AspNetCore.Mvc;
using NG.B2B.Business.Contract;
using System;
using System.Threading.Tasks;

namespace NG.B2B.Presentation.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        /// <summary>
        /// Retrieve a boolean indicating if the Coupon could be validated
        /// </summary>
        /// <param name="Id">The Id of the Coupon to validate</param>
        /// <remarks>
        /// ## Response code meanings
        /// - 200 - Tour successfully retrieved
        /// - 500 - An internal server error. Something bad and unexpected happened.
        /// - 543 - An internal server error. Something bad and unexpected happened.
        /// </remarks>
        /// <returns>A bool</returns>
        [HttpGet("{Id}")]
        public async Task<IActionResult> Validate(Guid Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _couponService.ValidateAsync(Id);

            return Ok(response);
        }
    }
}
