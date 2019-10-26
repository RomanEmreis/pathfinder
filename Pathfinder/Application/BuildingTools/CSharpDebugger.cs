using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Pathfinder.Application.BuildingTools {
    internal sealed class CSharpDebugger : IDebugger {
        public async Task<ScriptState<object>?> DebugAsync(CSharpDebuggingContext context) {
            if (context is null)                throw new ArgumentNullException(nameof(context));
            if (context.SourceText.Length == 0) return default;

            var options        = CreateDefaultOptions();
            var debuggingBlock = FetchDebuggingBlocks(context);
            var debuggerState  = default(ScriptState<object>);

            foreach (TextLineDebugger line in debuggingBlock) {
                debuggerState = await line.DebugAsync(debuggerState, options);
                context.CurrentLine++;
            }

            return debuggerState;
        }

        private IEnumerable<TextLine> FetchDebuggingBlocks(in CSharpDebuggingContext context) {
            return context.SourceText.Lines
                .Skip(context.CurrentLine)
                .Take(context.Breakpoint == 0 ? 1 : context.Breakpoint - 1);
        }

        private ScriptOptions CreateDefaultOptions() =>
            ScriptOptions.Default
                .WithImports("System")
                .WithEmitDebugInformation(true);
    }
}
