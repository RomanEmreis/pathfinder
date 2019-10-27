using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using Pathfinder.Application.BuildingTools.Background;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    public class CSharpDebuggingSession : IDebuggingSession {
        private          bool                 _isDisposed    = false;

        private          IDebugger            _debugger      = default!;
        private          ScriptState<object>? _debuggerState = default;
        private          SourceText           _sourceText    = default!;

        private          int                  _currentLine    = 0;
        private          int                  _nextBreakpoint = 0;

        public CSharpDebuggingSession(IDebugger debugger) {
            _debugger = debugger;
        }

        public async Task<BuildingResult> DebugAsync(BuildingTask buildingTask) {
            CheckDisposed();
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
            await _debugger.DebugAsync(debuggingContext);

            _debuggerState = debuggingContext.DebuggingState;
            _currentLine   = debuggingContext.CurrentLine;

            if (_currentLine == _sourceText.Lines.Count)
                Reset();

            return new BuildingResult { Success = true, CurrentLine = _currentLine, ResultMessage = sw.ToString() };
        }

        private CSharpDebuggingContext CreateDebuggingContext() => 
            new CSharpDebuggingContext(
                _sourceText, 
                _nextBreakpoint, 
                _currentLine, 
                default);

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

        private void CheckDisposed() {
            if (_isDisposed) throw new ObjectDisposedException(nameof(CSharpDebuggingSession));
        }

        public void Dispose() {
            if (_isDisposed) return;

            Reset();
            _debugger   = default!;
            _isDisposed = true;
        }
    }
}
