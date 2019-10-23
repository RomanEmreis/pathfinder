using Pathfinder.Application.BuildingTools.Background;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    public interface ICompiler {
        Task<byte[]> CompileAsync(BuildingTask buildingTask);
    }
}