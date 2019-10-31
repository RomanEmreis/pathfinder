using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;

namespace Pathfinder.Application.BuildingTools {
    /// <summary>
    ///     Represents the debugging context
    /// </summary>
    public interface IDebuggingContext {
        /// <summary>
        ///     Debugging source code
        /// </summary>
        SourceText SourceText { get; }

        /// <summary>
        ///     Positions of breakpoint
        /// </summary>
        int Breakpoint { get; }

        /// <summary>
        ///     Number of last code line
        /// </summary>
        int CurrentLine { get; set; }

        /// <summary>
        ///     Information of last debugging state
        /// </summary>
        ScriptState<object>? DebuggingState { get; set; }
    }
}
