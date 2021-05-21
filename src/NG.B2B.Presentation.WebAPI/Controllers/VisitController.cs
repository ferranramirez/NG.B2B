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
    [Authorize(Roles = "Commerce, Admin")]
    [Route("[controller]")]
    public class VisitController : ControllerBase
    {
        private readonly IVisitService _visitService;
        public VisitController(IVisitService visitService)
        {
            _visitService = visitService;
        }

        /// <summary>
        /// Return all the Visits for the given commerce.
        /// </summary>
        /// <param name="CommerceId">The Id of the commerce when want to check the visits from</param>
        /// <param name="AuthUserId">This value is ignored. The userId is constructed from the authorization token</param>
        /// <remarks>
        /// ## Response code meanings
        /// - 200 - Visits successfully retrieved.
        /// - 400 - The model is not properly built.
        /// - 500 - An internal server error. Something bad and unexpected happened.
        /// - 543 - A handled error. This error was expected, check the message.
        /// </remarks>
        /// <returns>A bool</returns>
        [AuthUserIdFromToken]
        [HttpGet("{CommerceId}")]
        [ProducesResponseType(typeof(ApiError), 543)]
        [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetByCommerce(Guid CommerceId,
            Guid AuthUserId = default /* Got from the [AuthUserIdFromToken] filter */)
        {
            return Ok(await _visitService.GetByCommerce(CommerceId, AuthUserId));
        }
    }
}
