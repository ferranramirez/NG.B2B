using Microsoft.Extensions.Options;
using NG.B2B.Business.Contract;
using NG.Common.Library.Exceptions;
using NG.DBManager.Infrastructure.Contracts.Models;
using NG.DBManager.Infrastructure.Contracts.UnitsOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NG.B2B.Business.Impl
{
    public class CouponService : ICouponService
    {
        public readonly IB2BUnitOfWork _unitOfWork;
        private readonly Dictionary<BusinessErrorType, BusinessErrorObject> _errors;

        public CouponService(IB2BUnitOfWork unitOfWork,
            IOptions<Dictionary<BusinessErrorType, BusinessErrorObject>> errors)
        {
            _unitOfWork = unitOfWork;
            _errors = errors.Value;
        }

        public async Task<Coupon> ValidateAsync(Guid couponId, Guid commerceUserId)
        {
            var coupon = _unitOfWork.Coupon.Get(couponId);

            if (coupon == null)
            {
                var error = _errors[BusinessErrorType.CouponNotFound];
                throw new NotGuiriBusinessException(error.Message, error.ErrorCode);
            }

            var commerceUserIdIsRight = _unitOfWork.Coupon
                .Find(c => c.Id == couponId
                    && c.Commerce.UserId == commerceUserId)
                .Any();

            if (!commerceUserIdIsRight)
            {
                var error = _errors[BusinessErrorType.WrongCommerce];
                throw new NotGuiriBusinessException(error.Message, error.ErrorCode);
            }

            if (coupon.IsValidated)
            {
                var error = _errors[BusinessErrorType.AlreadyValidatedCoupon];
                throw new NotGuiriBusinessException(error.Message, error.ErrorCode);
            }

            var currentDate = DateTime.Now;
            if (coupon.GenerationDate < currentDate.AddHours(-24))
            {
                var error = _errors[BusinessErrorType.ExpiredCoupon];
                throw new NotGuiriBusinessException(error.Message, error.ErrorCode);
            }

            coupon.ValidationDate = currentDate;
            _unitOfWork.Coupon.Update(coupon);
            await _unitOfWork.CommitAsync();

            return _unitOfWork.Coupon.Get(couponId);
        }
    }
}
