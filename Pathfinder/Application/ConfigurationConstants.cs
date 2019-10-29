namespace Pathfinder.Application {
    internal static class ConfigurationConstants {
        internal const string SqlConnection        = "DefaultConnection",
                              GoogleAuthSection    = "Authentication:Google",
                              MicrosoftAuthSection = "Authentication:Microsoft",
                              ClientId             = nameof(ClientId),
                              ClientSecret         = nameof(ClientSecret);
    }
}