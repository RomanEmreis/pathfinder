using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using static Pathfinder.Application.ConfigurationConstants;

namespace Pathfinder.Application.Extensions {
    internal static class OAuthExtensions {
        internal static void SetupSecrets(this OAuthOptions options, IConfigurationSection authConfigurationSection) {
            options.ClientId     = authConfigurationSection[ClientId];
            options.ClientSecret = authConfigurationSection[ClientSecret];
        }
    }
}