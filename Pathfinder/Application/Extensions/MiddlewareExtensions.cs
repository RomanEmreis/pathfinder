﻿using Microsoft.AspNetCore.Authentication.OAuth;
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
                    options.UseInMemoryDatabase("PathfinderDb")/*.UseSqlServer(conectionString)*/);
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
            var gitHubAuthSection    = configuration.GetSection(GitHubAuthSection);

            services
                .AddAuthentication()
                .AddGoogle(options           => options.SetupSecrets(googleAuthSection))
                .AddMicrosoftAccount(options => options.SetupSecrets(microsoftAuthSection))
                .AddGitHub(options           => options.SetupSecrets(gitHubAuthSection));

            return services;
        }
    }
}