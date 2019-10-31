using Pathfinder.Application.BuildingTools.Background;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    /// <summary>
    ///     Represents the hub of actual started debugging sessions
    /// </summary>
    public interface IDebuggingSessions {
        /// <summary>
        ///     Gets or adds debugging sessions by <paramref name="buildingTask"/>
        ///     and returns the instance of new or existed <see cref="IDebuggingSession"/>
        /// </summary>
        /// <param name="buildingTask">Description of the building operation</param>
        /// <returns>instance of new or existed <see cref="IDebuggingSession"/></returns>
        /// <exception cref="System.ArgumentNullException"/>
        ValueTask<IDebuggingSession> GetOrAddSession(BuildingTask buildingTask);

        /// <summary>
        ///     Ends the <see cref="IDebuggingSession"/> assosiated with the <paramref name="projectName"/>
        /// </summary>
        /// <param name="projectName">Name of the project that session needs to be ended</param>
        ValueTask EndSession(string projectName);
    }
}