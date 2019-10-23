using Pathfinder.Application.BuildingTools.Background;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    public interface IRunner {
        Task<BuildingResult> ExecuteAsync(byte[] compiledAssembly, string[] arguments);
    }
}