using Pathfinder.Application.BuildingTools.Background;
using System;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    public interface IDebuggingSession : IDisposable {
        Task<BuildingResult> DebugAsync(BuildingTask buildingTask);

        void Reset();
    }
}