﻿namespace Pathfinder.Application {
    internal static class RoutingConstants {
        internal const string DefaultRouteName    = "default",
                              DefaultRoutePattern = "{controller=Home}/{action=Index}/{id?}",
                              DefaultErrorRoute   = "/Home/Error",
                              ProjectsHubRoute    = "/projects",
                              EditorHubRoute      = "/liveeditor";
    }
}
