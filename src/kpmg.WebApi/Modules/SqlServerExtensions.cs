#region

using kpmg.Infrastructure.DataAccess;
using kpmg.WebApi.Modules.Common.FeatureFlags;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

#endregion

namespace kpmg.WebApi.Modules
{
    /// <summary>
    ///     Persistence Extensions.
    /// </summary>
    public static class SqlServerExtensions
    {
        /// <summary>
        ///     Add Persistence dependencies varying on configuration.
        /// </summary>
        public static IServiceCollection AddSqlServer(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            IFeatureManager featureManager = services
                .BuildServiceProvider()
                .GetRequiredService<IFeatureManager>();

            var isEnabled = featureManager
                .IsEnabledAsync(nameof(CustomFeature.SqlServer))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (isEnabled)
            {
                services.AddDbContext<KpmgContext>(
                    options => options.UseSqlServer(
                        configuration.GetValue<string>("PersistenceModule:DefaultConnection")));
            }

            return services;
        }
    }
}