using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Pathfinder.Application.BuildingTools {
    internal sealed class CSharpDebugger : IDebugger {
        private const string _systemImport = "System";

        public async Task DebugAsync(IDebuggingContext context) {
            if (context is null)                throw new ArgumentNullException(nameof(context));
            if (context.SourceText.Length == 0) return;

            var options        = CreateDefaultOptions();
            var debuggingBlock = FetchDebuggingBlocks(context);

            foreach (TextLineDebugger line in debuggingBlock) {
                context.DebuggingState = await line.DebugAsync(context.DebuggingState, options);
                context.CurrentLine++;
            }
        }

        private IEnumerable<TextLine> FetchDebuggingBlocks(IDebuggingContext context) {
            var breakpoint     = context.Breakpoint;
            var currentLine    = context.CurrentLine;

            var codeLinesRange = breakpoint == 0 ? 1 : breakpoint - currentLine - 1;

            return context.SourceText.Lines
                .Skip(currentLine)
                .Take(codeLinesRange);
        }

        private ScriptOptions CreateDefaultOptions() =>
            ScriptOptions.Default
                .WithImports(_systemImport)
                .WithEmitDebugInformation(true);
    }
}
