using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace Pathfinder.Application.BuildingTools {
    public sealed class CSharpDebuggingContext {
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
