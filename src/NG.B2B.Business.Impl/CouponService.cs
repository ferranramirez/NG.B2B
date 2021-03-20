using Microsoft.Extensions.Options;
using NG.B2B.Business.Contract;
using NG.Common.Library.Exceptions;
using NG.DBManager.Infrastructure.Contracts.Entities;
using NG.DBManager.Infrastructure.Contracts.Models;
using NG.DBManager.Infrastructure.Contracts.Models.Enums;
using NG.DBManager.Infrastructure.Contracts.UnitsOfWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NG.B2B.Business.Impl
{
    public class CouponService : ICouponService
    {
        public readonly IB2BUnitOfWork _unitOfWork;
        private readonly Dictionary<BusinessErrorType, BusinessErrorObject> _errors;

        public CouponService(
            IB2BUnitOfWork unitOfWork,
            IOptions<Dictionary<BusinessErrorType, BusinessErrorObject>> errors)
        {
            _unitOfWork = unitOfWork;
            _errors = errors.Value;
        }

        public async Task<Coupon> ValidateAsync(Guid couponId, Guid authUserId)
        {
            var coupon = _unitOfWork.Coupon.Get(couponId);

            if (coupon == null)
            {
                var error = _errors[BusinessErrorType.CouponNotFound];
                throw new NotGuiriBusinessException(error.Message, error.ErrorCode);
            }

            var commerce = _unitOfWork.Coupon.GetCommerce(couponId);
            var user = _unitOfWork.User.Get(authUserId);

            var isAdmin = user.Role == Role.Admin;
            var isRightCommerce = user.Role == Role.Commerce && commerce?.UserId == authUserId;
            var isRightUser = coupon.UserId == authUserId;
            var wrongCommerce = !(isAdmin || isRightCommerce || isRightUser);

            if (wrongCommerce)
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

            coupon.ValidatorId = authUserId;
            coupon.ValidationDate = currentDate;
            _unitOfWork.Coupon.Update(coupon);
            await _unitOfWork.CommitAsync();

            return _unitOfWork.Coupon.Get(couponId);
        }


        public async Task<IEnumerable<CouponInfo>> GetByCommerce(Guid commerceId, Guid commerceUserId, Guid authUserId)
        {
            var user = _unitOfWork.User.Get(authUserId);

            var wrongCommerce = !(user.Role == Role.Admin || (user.Role == Role.Commerce && commerceUserId == authUserId));

            if (wrongCommerce)
            {
                bool? containsCommerce = _unitOfWork.User.ContainsCommerce(commerceUserId, commerceId);

                if (containsCommerce == null)
                {
                    var error = _errors[BusinessErrorType.WrongData];
                    throw new NotGuiriBusinessException(error.Message, error.ErrorCode);
                }

                if (containsCommerce.Value)
                {
                    var error = _errors[BusinessErrorType.WrongCommerce];
                    throw new NotGuiriBusinessException(error.Message, error.ErrorCode);
                }
            }

            return await _unitOfWork.Coupon.GetByCommerce(commerceId);
        }

    }
}
