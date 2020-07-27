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
        private Guid commerceUserId;
        private Guid nodeId;

        public CouponServiceTests()
        {
            couponId = Guid.NewGuid();
            commerceUserId = Guid.NewGuid();
            nodeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            coupon = new Coupon()
            {
                Id = couponId,
                NodeId = nodeId,
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

        [Fact(Skip = "To be fixed or removed")]
        public async Task Validate_GivesRightCoupon_ReturnsTrue()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Coupon.Get(couponId)).Returns(coupon);

            var commerces = new List<Commerce>
            {
                new Commerce() { Id = Guid.NewGuid(), Name = "Test Commerce" }
            };
            _unitOfWorkMock.Setup(uow => uow.Commerce.Find(c => c.UserId == commerceUserId)).Returns(commerces);

            // Act
            var actual = await _couponService.ValidateAsync(couponId, commerceUserId);

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public async Task Validate_GivesWrongCouponId_ThrowsCouponNotFound()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Coupon.Get(couponId)).Returns((Coupon)null);
            coupon.ValidationDate = DateTime.Now;

            // Act
            async Task action() => await _couponService.ValidateAsync(couponId, commerceUserId);

            // Assert
            var exception = await Assert.ThrowsAsync<NotGuiriBusinessException>(action);
            Assert.Equal(104, exception.ErrorCode);
        }

        [Fact]
        public async Task Validate_GivesWrongCommerceId_ThrowsWrongCommerce()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Coupon.Get(couponId)).Returns(coupon);

            var commerces = new List<Commerce>();
            _unitOfWorkMock.Setup(uow => uow.Commerce.Find(c => c.UserId == commerceUserId)).Returns(commerces);

            // Act
            async Task action() => await _couponService.ValidateAsync(couponId, commerceUserId: Guid.Empty);

            // Assert
            var exception = await Assert.ThrowsAsync<NotGuiriBusinessException>(action);
            Assert.Equal(105, exception.ErrorCode);
        }

        [Fact(Skip = "To be fixed or removed")]
        public async Task Validate_GivesAlreadyValidatedCoupon_ThrowsAlreadyValidatedCoupon()
        {
            // Arrange
            coupon.ValidationDate = DateTime.Now;
            _unitOfWorkMock.Setup(uow => uow.Coupon.Get(couponId)).Returns(coupon);
            var coupons = new List<Coupon> { coupon };
            //_unitOfWorkMock.Setup(uow => uow.Coupon
            //    .Find(c => c.Id == couponId
            //        && c.Commerce.UserId == commerceUserId)).Returns(coupons);

            // Act
            async Task action() => await _couponService.ValidateAsync(couponId, commerceUserId).ConfigureAwait(false);

            // Assert
            var exception = await Assert.ThrowsAsync<NotGuiriBusinessException>(action);
            Assert.Equal(106, exception.ErrorCode);
        }

        [Fact(Skip = "To be fixed or removed")]
        public async Task Validate_GivesExpiredCoupon_ThrowsExpiredCoupon()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.Coupon.Get(couponId)).Returns(coupon);
            coupon.GenerationDate = coupon.GenerationDate.AddMonths(-2);
            var coupons = new List<Coupon> { coupon };
            //_unitOfWorkMock.Setup(uow => uow.Coupon
            //    .Find(c => c.Id == couponId
            //        && c.Commerce.UserId == commerceUserId)).Returns(coupons);

            // Act
            async Task action() => await _couponService.ValidateAsync(couponId, commerceUserId).ConfigureAwait(false);

            // Assert
            var exception = await Assert.ThrowsAsync<NotGuiriBusinessException>(action);
            Assert.Equal(107, exception.ErrorCode);
        }
    }
}
