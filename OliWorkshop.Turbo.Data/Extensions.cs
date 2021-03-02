using Microsoft.EntityFrameworkCore;
using OliWorkshop.Turbo.Data;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceCollection AddRepository<TContext>(this IServiceCollection services)
        where TContext : DbContext
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // register data service repository
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<TContext, dynamic>));
            return services;
        }

        public static IServiceCollection AddDbSetup<TContext>(this IServiceCollection services,
            DbSetupParameters parameters,
            IDatabaseSetup setup,
            bool devMode = false,
            bool poolMode = false)
        where TContext : DbContext 
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (poolMode)
            {
                services.AddDbContextPool<TContext>(builder => {
                    setup.DbSetup(parameters, builder);
                    EnableDevFeatures(builder, devMode);
                });
            }
            else
            {
                services.AddDbContext<TContext>(builder => {
                    setup.DbSetup(parameters, builder);
                    EnableDevFeatures(builder, devMode);
                });
            }
            return services;
        }

        private static void EnableDevFeatures(DbContextOptionsBuilder builder, bool devMode)
        {
            if (devMode)
            {
                builder.EnableDetailedErrors();
                builder.EnableSensitiveDataLogging();
                builder.ConfigureWarnings(x => {
                    x.Default(WarningBehavior.Log);
                });
            }
            else
            {
                builder.EnableServiceProviderCaching(true);
                builder.ConfigureWarnings(x => {
                    x.Default(WarningBehavior.Ignore);
                });
            }
        }
    }
}
