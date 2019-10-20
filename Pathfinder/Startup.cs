using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pathfinder.Application.Extensions;
using Pathfinder.Application.Hubs;
using Pathfinder.Application.Services;
using static Pathfinder.Application.RoutingConstants;

namespace Pathfinder {
    public class Startup {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services
                .ConfigureSql(Configuration)
                .ConfigureIdentity()
                .ConfigureAuthentication(Configuration);

            services.AddSingleton<IProjectService, ProjectService>();
            services.AddSingleton<ICollaboratorsService, CollaboratorsService>();

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name:    DefaultRouteName,
                    pattern: DefaultRoutePattern);
                endpoints.MapRazorPages();
                endpoints.MapHub<ProjectsHub>("/projects");
                endpoints.MapHub<LiveEditorHub>("/liveeditor");
            });
        }
    }
}