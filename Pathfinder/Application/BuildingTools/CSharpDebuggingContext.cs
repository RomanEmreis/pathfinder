using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;

namespace Pathfinder.Application.BuildingTools {
    /// <summary>
    ///     Implementation of <see cref="IDebuggingContext"/> for C# language
    /// </summary>
    public sealed class CSharpDebuggingContext : IDebuggingContext {
        public CSharpDebuggingContext(SourceText sourceText, int breakpoint, int currentLine, ScriptState<object>? debuggingState) {
            SourceText  = sourceText;
            Breakpoint  = breakpoint;
            CurrentLine = currentLine;
            DebuggingState = debuggingState;
        }

        public SourceText SourceText { get; }

        public int Breakpoint { get; }

        public int CurrentLine { get; set; }

        public ScriptState<object>? DebuggingState { get; set; }
    }
}
