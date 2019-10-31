using Pathfinder.Application.BuildingTools.Background;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    /// <summary>
    ///     Represents an abstract compiler
    /// </summary>
    public interface ICompiler {
        /// <summary>
        ///     Compile the code by given <paramref name="buildingTask"/>
        ///     and returns the bytes of compiled assembly
        /// </summary>
        /// <param name="buildingTask">Information for compilation</param>
        /// <returns>bytes of compiled assembly</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        Task<byte[]> CompileAsync(BuildingTask buildingTask);
    }
}