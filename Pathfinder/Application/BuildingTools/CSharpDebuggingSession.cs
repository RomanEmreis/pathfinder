using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.DependencyInjection;
using Pathfinder.Application.BuildingTools.Background;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    public class CSharpDebuggingSession : IDebuggingSession {
        private readonly IDebugger            _debugger;
        private          ScriptState<object>? _debuggerState = default;
        private          SourceText           _sourceText    = default!;

        private          int                  _currentLine    = 0;
        private          int                  _nextBreakpoint = 0;

        public CSharpDebuggingSession(IDebugger debugger) {
            _debugger = debugger;
        }

        public async Task<BuildingResult> DebugAsync(BuildingTask buildingTask) {
            if (buildingTask is null) throw new ArgumentNullException(nameof(buildingTask));

            var assemblyName = buildingTask.ProjectName;
            var sourceCode   = buildingTask.SourceCode;

            if (string.IsNullOrWhiteSpace(assemblyName) || string.IsNullOrWhiteSpace(sourceCode))
                throw new InvalidOperationException(ErrorConstants.MissingParametersMessage);

            _sourceText = SourceText.From(sourceCode);

            using var sw = new StringWriter();
            Console.SetOut(sw);
            Console.SetError(sw);

            _nextBreakpoint      = NextBreakpoint(buildingTask);

            var debuggingContext = CreateDebuggingContext();
            _debuggerState       = await _debugger.DebugAsync(debuggingContext);

            _currentLine         = debuggingContext.CurrentLine;

            if (_currentLine == _sourceText.Lines.Count)
                Reset();

            return new BuildingResult { Success = true, CurrentLine = _currentLine, ResultMessage = sw.ToString() };
        }

        private CSharpDebuggingContext CreateDebuggingContext() => 
            new CSharpDebuggingContext(_sourceText, _nextBreakpoint, _currentLine);

        private int NextBreakpoint(BuildingTask buildingTask) {
            var currentLine = buildingTask.CurrentLine;
            var nextBreakpoint = buildingTask
                .Breakpoints
                .FirstOrDefault(breakpoint => breakpoint > currentLine);

            return buildingTask.IsRunToNextBreakpoint 
                ? nextBreakpoint == 0 ? _sourceText.Lines.Count : nextBreakpoint
                : 0;
        }

        public void Reset() {
            _debuggerState  = default;
            _sourceText     = default!;
            _currentLine    = 0;
            _nextBreakpoint = 0;
        }
    }
}
