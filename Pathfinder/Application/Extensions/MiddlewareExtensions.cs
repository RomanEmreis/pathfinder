using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Data;
using static Pathfinder.Application.ConfigurationConstants;

namespace Pathfinder.Application.Extensions {
    internal static class MiddlewareExtensions {
        internal static IServiceCollection ConfigureSql(this IServiceCollection services, IConfiguration configuration) {
            var conectionString = configuration.GetConnectionString(SqlConnection);
            return services
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(conectionString));
        }

        internal static IServiceCollection ConfigureIdentity(this IServiceCollection services) {
            services
                .AddDefaultIdentity<IdentityUser>(options => 
                    options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            return services;
        }

        internal static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration) {
            var googleAuthSection    = configuration.GetSection(GoogleAuthSection);
            var microsoftAuthSection = configuration.GetSection(MicrosoftAuthSection);
            services
                .AddAuthentication()
                .AddGoogle(options => {
                    options.ClientId      = googleAuthSection[ClientId];
                    options.ClientSecret  = googleAuthSection[ClientSecret];
                })
                .AddMicrosoftAccount(options => {
                    options.ClientId     = microsoftAuthSection[ClientId];
                    options.ClientSecret = microsoftAuthSection[ClientSecret];
                });
            return services;
        }
    }


}