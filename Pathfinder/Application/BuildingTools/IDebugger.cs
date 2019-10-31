using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    /// <summary>
    ///     Represents an abstract debugger
    /// </summary>
    public interface IDebugger {
        /// <summary>
        ///     Debug code in a given context
        /// </summary>
        /// <param name="context">source code and auxilary information for debug</param>
        /// <returns></returns>
        Task DebugAsync(IDebuggingContext context);
    }
}