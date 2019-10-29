using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Pathfinder.Application.BuildingTools {
    internal sealed class CSharpDebugger : IDebugger {
        public async Task DebugAsync(CSharpDebuggingContext context) {
            if (context is null)                throw new ArgumentNullException(nameof(context));
            if (context.SourceText.Length == 0) return;

            var options        = CreateDefaultOptions();
            var debuggingBlock = FetchDebuggingBlocks(context);

            foreach (TextLineDebugger line in debuggingBlock) {
                context.DebuggingState = await line.DebugAsync(/*context.DebuggingState*/default, options);
                context.CurrentLine++;
            }
        }

        private IEnumerable<TextLine> FetchDebuggingBlocks(CSharpDebuggingContext context) {
            var breakpoint  = context.Breakpoint;
            var currentLine = context.CurrentLine;

            return context.SourceText.Lines
                .Skip(currentLine)
                .Take(breakpoint == 0 ? 1 : breakpoint - currentLine - 1);
        }

        private ScriptOptions CreateDefaultOptions() =>
            ScriptOptions.Default
                .WithImports("System")
                .WithEmitDebugInformation(true);
    }
}
