using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pathfinder.Application.Hubs;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools.Background {
    internal sealed class BuildingService : BackgroundService {
        private readonly IBuildingQueue       _buildingQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IDebuggingSessions   _debuggingSessions;

        public BuildingService(IBuildingQueue buildingQueue, IDebuggingSessions debuggingSessions, IServiceScopeFactory serviceScopeFactory) {
            _buildingQueue       = buildingQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _debuggingSessions   = debuggingSessions;
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken) {
            using var scope = _serviceScopeFactory.CreateScope();

            while (!cancellationToken.IsCancellationRequested) {
                var buildingTask = await _buildingQueue.DequeueAsync(cancellationToken);
                if (buildingTask.IsDebugging) {
                    await RunDebug(scope, buildingTask);
                } else {
                    await RunToCompletion(scope, buildingTask);
                }
            }
        }

        private async Task RunToCompletion(IServiceScope scope, BuildingTask buildingTask) {
            var compiler         = scope.ServiceProvider.GetRequiredService<ICompiler>();
            var runner           = scope.ServiceProvider.GetRequiredService<IRunner>();
            var editorHubContext = scope.ServiceProvider.GetRequiredService<IHubContext<LiveEditorHub>>();

            var projectName = buildingTask.ProjectName;
            var buildingResult = default(BuildingResult);

            using var sw = new StringWriter();
            try {
                Console.SetOut(sw);
                Console.SetError(sw);

                var assemblyBytes = await compiler.CompileAsync(buildingTask);
                buildingResult = await runner.ExecuteAsync(assemblyBytes, Array.Empty<string>());

                buildingResult.ResultMessage = sw.ToString();
            } catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentNullException) {
                buildingResult = new BuildingResult {
                    ResultMessage = ex.Message
                };
            } finally {
                await editorHubContext
                    .Clients
                    .Group(projectName)
                    .SendAsync(nameof(LiveEditorHub.ProjectCompiled), projectName, buildingResult);
            }
        }

        private async Task RunDebug(IServiceScope scope, BuildingTask buildingTask) {
            var debuggingSession = await _debuggingSessions.GetOrAddSession(buildingTask);
            var editorHubContext = scope.ServiceProvider.GetRequiredService<IHubContext<LiveEditorHub>>();

            var buildingResult = await debuggingSession.DebugAsync(buildingTask);

            await editorHubContext
                .Clients
                .Group(buildingTask.ProjectName)
                .SendAsync(nameof(LiveEditorHub.StepOver), buildingResult);
        }
    }
}