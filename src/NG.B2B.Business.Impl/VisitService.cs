using Microsoft.Extensions.Options;
using NG.B2B.Business.Contract;
using NG.Common.Library.Exceptions;
using NG.DBManager.Infrastructure.Contracts.Entities;
using NG.DBManager.Infrastructure.Contracts.Models.Enums;
using NG.DBManager.Infrastructure.Contracts.UnitsOfWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NG.B2B.Business.Impl
{
    public class VisitService : IVisitService
    {
        public readonly IB2BUnitOfWork _unitOfWork;
        private readonly Dictionary<BusinessErrorType, BusinessErrorObject> _errors;

        public VisitService(
            IB2BUnitOfWork unitOfWork,
            IOptions<Dictionary<BusinessErrorType, BusinessErrorObject>> errors)
        {
            _unitOfWork = unitOfWork;
            _errors = errors.Value;
        }

        public async Task<IEnumerable<VisitInfo>> GetByCommerce(Guid commerceId, Guid authUserId)
        {
            var user = _unitOfWork.User.Get(authUserId);
            var commerce = _unitOfWork.Commerce.Get(commerceId);

            var wrongCommerce = !(user.Role == Role.Admin ||
                (user.Role == Role.Commerce && commerce?.UserId == authUserId));

            if (wrongCommerce)
            {
                var error = _errors[BusinessErrorType.WrongCommerce];
                throw new NotGuiriBusinessException(error.Message, error.ErrorCode);
            }

            return await _unitOfWork.Visit.GetByCommerce(commerceId);
        }
    }
}
