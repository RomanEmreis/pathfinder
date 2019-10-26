using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Threading.Tasks;

namespace Pathfinder.Application.BuildingTools {
    internal sealed class TextLineDebugger {
        private readonly TextLine _textLine;

        internal TextLineDebugger(in TextLine textLine) => _textLine = textLine;

        internal Script<object> GetScript(ScriptOptions options) => 
            CSharpScript.Create(_textLine.ToString(), options);

        internal async Task<ScriptState<object>?> DebugAsync(ScriptState<object>? lastState, ScriptOptions options) {
            var script = CSharpScript.Create(_textLine.ToString(), options);
            try {
                lastState = lastState is null 
                    ? await script.RunAsync()
                    : await script.RunFromAsync(lastState);
            } catch (CompilationErrorException) {
            } catch (ArgumentException) {
            } catch (Exception ex) {
                Console.Error.WriteLine(ex.InnerException ?? ex);
            }

            return lastState;
        }

        public static implicit operator TextLineDebugger(in TextLine textLine) => new TextLineDebugger(in textLine);
    }
}