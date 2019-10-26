using Pathfinder.Application.BuildingTools.Background;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    public interface IDebuggingSessions {
        ValueTask<IDebuggingSession> GetOrAddSession(BuildingTask buildingTask);
    }
}