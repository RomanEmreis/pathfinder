using Pathfinder.Application.BuildingTools.Background;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    public interface IDebuggingSession {
        Task<BuildingResult> DebugAsync(BuildingTask buildingTask);

        void Reset();
    }
}