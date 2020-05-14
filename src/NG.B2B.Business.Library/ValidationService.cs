using NG.B2B.Business.Contract;
using NG.DBManager.Infrastructure.Contracts.Models;
using NG.DBManager.Infrastructure.Contracts.UnitsOfWork;
using System;

namespace NG.B2B.Business.Library
{
    public class CouponService : ICouponService
    {
        public readonly IUnitOfWork _unitOfWork;
        public CouponService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool Validate(Guid couponId)
        {
            var existingCoupon = _unitOfWork.Repository<Coupon>().Get(couponId);

            if (existingCoupon != null)
            {
                return false;
            }

            if (existingCoupon.IsRedeemed)
            {
                //throw new Exception("Coupon already redeemed");             
                return false;
            }
            // if existingCoupon.Created > 24DaysAgo then false
            existingCoupon.Redemption = DateTime.Now;

            return _unitOfWork.Commit() == 1;
        }
    }
}
