namespace Pathfinder.Application {
    internal static class ConfigurationConstants {
        internal const string SqlConnection        = "DefaultConnection",
                              GoogleAuthSection    = "Authentication:Google",
                              MicrosoftAuthSection = "Authentication:Microsoft",
                              GitHubAuthSection    = "Authentication:GitHub",
                              ClientId             = nameof(ClientId),
                              ClientSecret         = nameof(ClientSecret);
    }
}