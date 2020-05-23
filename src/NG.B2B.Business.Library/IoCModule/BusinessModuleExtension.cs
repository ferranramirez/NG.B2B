using Microsoft.Extensions.DependencyInjection;
using NG.B2B.Business.Contract;
using NG.DBManager.Infrastructure.Contracts.UnitsOfWork;
using NG.DBManager.Infrastructure.Impl.EF.IoCModule;
using NG.DBManager.Infrastructure.Impl.EF.UnitsOfWork;
using System;

namespace NG.B2B.Business.Library.IoCModule
{
    public static class BusinessModuleExtension
    {
        public static IServiceCollection AddBusinessServices(
           this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddInfrastructureServices()
                    .AddScoped<IB2BUnitOfWork, B2BUnitOfWork>()
                    .AddScoped<ICouponService, CouponService>();

            return services;
        }
    }
}
