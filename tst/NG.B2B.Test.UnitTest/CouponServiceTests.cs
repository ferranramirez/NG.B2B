using Microsoft.Extensions.Options;
using Moq;
using NG.B2B.Business.Contract;
using NG.B2B.Business.Impl;
using NG.Common.Library.Exceptions;
using NG.DBManager.Infrastructure.Contracts.Models;
using NG.DBManager.Infrastructure.Contracts.UnitsOfWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NG.B2B.Test.UnitTest
{
    public class CouponServiceTests
    {
        private Mock<IB2BUnitOfWork> _unitOfWorkMock;
        private ICouponService _couponService;
        private Coupon coupon;
        private Guid couponId;
        private Guid commerceId;

        public CouponServiceTests()
        {
            couponId = Guid.NewGuid();
            commerceId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            coupon = new Coupon()
            {
                Id = couponId,
                CommerceId = commerceId,
                UserId = userId,
                Content = "{ Coupon content }",
                GenerationDate = DateTime.Now
            };

            _unitOfWorkMock = new Mock<IB2BUnitOfWork>();

            var errorsDictionary = new Dictionary<BusinessErrorType, BusinessErrorObject>
            {
                { BusinessErrorType.CouponNotFound, new BusinessErrorObject() { ErrorCode = 104, Message = "Error test" } },
                { BusinessErrorType.WrongCommerce, new BusinessErrorObject() { ErrorCode = 105, Message = "Error test" } },
                { BusinessErrorType.AlreadyValidatedCoupon, new BusinessErrorObject() { ErrorCode = 106, Message = "Error test" } },
                { BusinessErrorType.ExpiredCoupon, new BusinessErrorObject() { ErrorCode = 107, Message = "Error test" } }
            };
            var _options = Options.Create(errorsDictionary);

            _couponService = new CouponService(_unitOfWorkMock.Object, _options);
        }

        [Fact]
        public async Task Validate_GivesRightCoupon_ReturnsTrue()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Coupon.Get(couponId)).Returns(coupon);
            _unitOfWorkMock.Setup(uow => uow.CommitAsync()).Returns(Task.FromResult(1));

            // Act
            var actual = await _couponService.ValidateAsync(couponId, commerceId);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public async Task Validate_GivesWrongCouponId_ThrowsCouponNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Coupon.Get(couponId)).Returns((Coupon)null);
            coupon.ValidationDate = DateTime.Now;

            // Act
            async Task action() => await _couponService.ValidateAsync(couponId, commerceId);

            // Assert
            var exception = await Assert.ThrowsAsync<NotGuiriBusinessException>(action);
            Assert.Equal(104, exception.ErrorCode);
        }

        [Fact]
        public async Task Validate_GivesWrongCommerceId_ThrowsWrongCommerce()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Coupon.Get(couponId)).Returns(coupon);
            coupon.ValidationDate = DateTime.Now;

            // Act
            async Task action() => await _couponService.ValidateAsync(couponId, commerceId: Guid.Empty);

            // Assert
            var exception = await Assert.ThrowsAsync<NotGuiriBusinessException>(action);
            Assert.Equal(105, exception.ErrorCode);
        }

        [Fact]
        public async Task Validate_GivesAlreadyValidatedCoupon_ThrowsAlreadyValidatedCoupon()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Coupon.Get(couponId)).Returns(coupon);
            coupon.ValidationDate = DateTime.Now;

            // Act
            async Task action() => await _couponService.ValidateAsync(couponId, commerceId).ConfigureAwait(false);

            // Assert
            var exception = await Assert.ThrowsAsync<NotGuiriBusinessException>(action);
            Assert.Equal(106, exception.ErrorCode);
        }

        [Fact]
        public async Task Validate_GivesExpiredCoupon_ThrowsExpiredCoupon()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Coupon.Get(couponId)).Returns(coupon);
            coupon.GenerationDate = coupon.GenerationDate.AddMonths(-2);

            // Act
            async Task action() => await _couponService.ValidateAsync(couponId, commerceId).ConfigureAwait(false);

            // Assert
            var exception = await Assert.ThrowsAsync<NotGuiriBusinessException>(action);
            Assert.Equal(107, exception.ErrorCode);
        }
    }
}
