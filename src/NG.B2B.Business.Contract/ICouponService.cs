using NG.DBManager.Infrastructure.Contracts.Models;
using System;
using System.Threading.Tasks;

namespace NG.B2B.Business.Contract
{
    public interface ICouponService
    {
        Task<Coupon> ValidateAsync(Guid couponId, Guid commerceUserId);
    }
}
