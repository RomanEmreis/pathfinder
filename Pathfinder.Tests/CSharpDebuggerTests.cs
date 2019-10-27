using FluentAssertions;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using Pathfinder.Application.BuildingTools;
using Pathfinder.Application.BuildingTools.Background;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Pathfinder.Tests {
    public class CSharpDebuggerTests {
        [Fact]
        public void DebugAsync_Null_CSharpDebuggingContext_Should_Throw_ArgumentNullException() {
            var debugger = new CSharpDebugger();
            Func<Task> act = () => debugger.DebugAsync(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData(6)]
        [InlineData(5)]
        [InlineData(4)]
        [InlineData(3)]
        [InlineData(2)]
        [InlineData(1)]
        public async Task StartDebug_GoodCode_With_Breakpoint_CurrentLine_Should_Be_Equals_To(int breakpoint) {
            var debugger = new CSharpDebugger();
            var sourceText = SourceText.From(CSharpSnippets.GoodCode);
            var context = new CSharpDebuggingContext(sourceText, breakpoint, 0, null);
            await debugger.DebugAsync(context);

            context.CurrentLine.Should().Be(breakpoint - 1);
        }
    }
}
