using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NG.B2B.Business.Contract;
using NG.Common.Library.Exceptions;
using NG.Common.Services.Token;
using NG.DBManager.Infrastructure.Contracts.UnitsOfWork;
using NG.DBManager.Infrastructure.Impl.EF.Extensions;
using NG.DBManager.Infrastructure.Impl.EF.UnitsOfWork;
using System;
using System.Collections.Generic;

namespace NG.B2B.Business.Impl.IoCModule
{
    public static class BusinessModuleExtension
    {
        public static IServiceCollection AddBusinessServices(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddInfrastructureServices()
                    .AddScoped<IB2BUnitOfWork, B2BUnitOfWork>()
                    .AddScoped<ICouponService, CouponService>()
                    .AddSingleton<ITokenService, TokenService>()
                    .Configure<Dictionary<BusinessErrorType, BusinessErrorObject>>(x => configuration.GetSection("Errors").Bind(x));

            return services;
        }
    }
}
