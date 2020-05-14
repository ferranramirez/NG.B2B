using System;

namespace NG.B2B.Business.Contract
{
    public interface ICouponService
    {
        bool Validate(Guid couponId);
    }
}
