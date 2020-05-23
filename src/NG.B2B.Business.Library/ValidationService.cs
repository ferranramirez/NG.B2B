using NG.B2B.Business.Contract;
using NG.DBManager.Infrastructure.Contracts.UnitsOfWork;
using System;
using System.Threading.Tasks;

namespace NG.B2B.Business.Library
{
    public class CouponService : ICouponService
    {
        public readonly IB2BUnitOfWork _unitOfWork;
        public CouponService(IB2BUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ValidateAsync(Guid couponId)
        {
            var existingCoupon = _unitOfWork.Coupon.Get(couponId);

            if (existingCoupon == null)
            {
                return false;
            }

            if (existingCoupon.IsValidated)
            {
                //throw new Exception("Coupon already redeemed");             
                return false;
            }
            // if existingCoupon.Created > 24DaysAgo then false
            existingCoupon.ValidationDate = DateTime.Now;

            return await _unitOfWork.CommitAsync() == 1;
        }
    }
}
