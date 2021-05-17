using NG.DBManager.Infrastructure.Contracts.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NG.B2B.Business.Contract
{
    public interface IVisitService
    {
        Task<IEnumerable<VisitInfo>> GetByCommerce(Guid commerceId, Guid commerceUserId, Guid authUserId);
    }
}
