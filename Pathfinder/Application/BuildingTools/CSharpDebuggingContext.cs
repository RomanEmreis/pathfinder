using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace Pathfinder.Application.BuildingTools {
    public sealed class CSharpDebuggingContext {
        public CSharpDebuggingContext(SourceText sourceText, int breakpoint, int currentLine) {
            SourceText  = sourceText;
            Breakpoint  = breakpoint;
            CurrentLine = currentLine;
        }

        public SourceText SourceText { get; }

        public int Breakpoint { get; }

        public int CurrentLine { get; set; }
    }
}
