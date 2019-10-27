using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Application.BuildingTools.Background;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    public sealed class CSharpDebuggingSessions : IDebuggingSessions {
        private readonly ConcurrentDictionary<string, IDebuggingSession> _sessions = new ConcurrentDictionary<string, IDebuggingSession>();
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CSharpDebuggingSessions(IServiceScopeFactory serviceScopeFactory) {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public ValueTask<IDebuggingSession> GetOrAddSession(BuildingTask buildingTask) {
            if (buildingTask is null) throw new ArgumentNullException(nameof(buildingTask));

            if (_sessions.TryGetValue(buildingTask.ProjectName, out var session)) {
                return new ValueTask<IDebuggingSession>(session);
            } else {
                var scope                  = _serviceScopeFactory.CreateScope();
                var debugger               = scope.ServiceProvider.GetService<IDebugger>();

                var csharpDebuggingSession = new CSharpDebuggingSession(debugger);
                _ = _sessions.TryAdd(buildingTask.ProjectName, csharpDebuggingSession);

                return new ValueTask<IDebuggingSession>(csharpDebuggingSession);
            }
        }

        public ValueTask EndSession(string projectName) {
            if (_sessions.TryRemove(projectName, out var removedSession))
                removedSession.Dispose();

            return new ValueTask();
        }
    }
}