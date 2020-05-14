using Microsoft.Extensions.DependencyInjection;
using NG.B2B.Business.Contract;
using NG.DBManager.Infrastructure.Impl.EF.IoCModule;
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
                    .AddScoped<ICouponService, CouponService>();

            return services;
        }
    }
}
