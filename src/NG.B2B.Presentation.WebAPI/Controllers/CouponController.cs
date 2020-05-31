using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NG.B2B.Business.Contract;
using NG.Common.Library.Filters;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NG.B2B.Presentation.WebAPI.Controllers
{
    [ApiController]
    [Authorize(Roles = "Commerce")]
    [Route("[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        /// <summary>
        /// Validate an already existing coupon
        /// </summary>
        /// <param name="Id">The Id of the Coupon to validate</param>
        /// <remarks>
        /// ## Response code meanings
        /// - 200 - Coupon successfully validated.
        /// - 400 - The model is not properly built.
        /// - 500 - An internal server error. Something bad and unexpected happened.
        /// - 543 - A handled error. This error was expected, check the message.
        /// </remarks>
        /// <returns>A bool</returns>
        [AuthUserIdFromToken]
        [HttpPut("{CouponId}")]
        [ProducesResponseType(typeof(ApiError), 543)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Validate(Guid CouponId,
            Guid AuthUserId = default /* Got from the [AuthUserIdFromToken] filter */)
        {
            return Ok(await _couponService.ValidateAsync(CouponId, AuthUserId));
        }
    }
}
