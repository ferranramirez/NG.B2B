using System;
using System.Threading.Tasks;

namespace NG.B2B.Business.Contract
{
    public interface ICouponService
    {
        Task<bool> ValidateAsync(Guid couponId);
    }
}
