using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Shared
{
    /// <summary>
    /// Extension to register BankingContext in Dependency Injection.
    /// </summary>
    public static class BankingContextExtensions
    {
        /// <summary>
        /// Adds BankingContext to the specified IServiceCollection. Uses the Sqlite database provider.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="relativePath">Set to override the default of ".."</param>
        /// <returns>An IServiceCollection that can be used to add more services.</returns>
        public static IServiceCollection AddBankingContext(
            this IServiceCollection services,
            string relativePath = ".."
        )
        {
            string databasePath = Path.Combine(relativePath, "Banking.db");
            services.AddDbContext<BankingContext>(options =>
                options.UseSqlite($"Data Source={databasePath}")
            );
            return services;
        }
    }
}
