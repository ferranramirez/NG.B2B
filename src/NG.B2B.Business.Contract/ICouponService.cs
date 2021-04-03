using NG.DBManager.Infrastructure.Contracts.Entities;
using NG.DBManager.Infrastructure.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NG.B2B.Business.Contract
{
    public interface ICouponService
    {
        Task<Coupon> ValidateAsync(Guid couponId, Guid authUserId);
        Task<IEnumerable<CouponInfo>> GetByCommerce(Guid commerceId, Guid CommerceUserId, Guid authUserId);
    }
}
