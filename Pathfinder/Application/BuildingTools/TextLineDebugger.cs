using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    /// <summary>
    ///     Represents the single line debugger
    /// </summary>
    internal sealed class TextLineDebugger {
        private readonly TextLine _textLine;

        internal TextLineDebugger(in TextLine textLine) => _textLine = textLine;

        internal Script<object> GetScript(ScriptOptions options) => 
            CSharpScript.Create(_textLine.ToString(), options);

        internal async Task<ScriptState<object>?> DebugAsync(ScriptState<object>? lastState, ScriptOptions options) {
            var scriptString = _textLine.ToString();
            if (string.IsNullOrWhiteSpace(scriptString)) return lastState;
            
            try {
                lastState = lastState is null
                    ? await CSharpScript.Create(scriptString, options).RunAsync()
                    : await lastState.ContinueWithAsync(scriptString, options);
            } catch (CompilationErrorException) {
            } catch (ArgumentException) {
                lastState = null;
            } catch (Exception ex) {
                Console.Error.WriteLine(ex.InnerException ?? ex);
            }

            return lastState;
        }

        public static implicit operator TextLineDebugger(in TextLine textLine) => new TextLineDebugger(in textLine);
    }
}