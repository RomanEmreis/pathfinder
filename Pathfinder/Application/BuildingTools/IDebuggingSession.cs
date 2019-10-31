using Pathfinder.Application.BuildingTools.Background;
using System;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    /// <summary>
    ///     Represents the session that holds information about debugging context
    /// </summary>
    public interface IDebuggingSession : IDisposable {
        /// <summary>
        ///     Debug code of <paramref name="buildingTask"/> and returns the <see cref="BuildingResult"/>
        /// </summary>
        /// <param name="buildingTask">description of debugging operation</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task<BuildingResult> DebugAsync(BuildingTask buildingTask);

        /// <summary>
        ///     Resets all parametes to defaults
        /// </summary>
        void Reset();
    }
}