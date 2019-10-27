using FluentAssertions;
using Pathfinder.Application.BuildingTools;
using Pathfinder.Application.BuildingTools.Background;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Pathfinder.Tests {
    public class CSharpDebuggingSessionTests {
        [Fact]
        public void DebugAsync_Null_BuildingTask_Should_Throw_ArgumentNullException() {
            Func<Task<BuildingResult>> act = () => new CSharpDebuggingSession(new CSharpDebugger()).DebugAsync(null);
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(" ", " ")]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(" ", null)]
        [InlineData(null, "")]
        [InlineData(null, " ")]
        [InlineData(null, "test")]
        [InlineData("", "test")]
        [InlineData(" ", "test")]
        [InlineData("test", "")]
        [InlineData("test", " ")]
        [InlineData("test", null)]
        public void DebugAsync_Empty_BuildingTask_Should_Throw_InvalidOperationException(string projectName, string sourceCode) {

            Func<Task<BuildingResult>> act = () => new CSharpDebuggingSession(new CSharpDebugger()).DebugAsync(new BuildingTask(projectName, sourceCode));
            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Theory]
        [InlineData(6)]
        [InlineData(5)]
        [InlineData(4)]
        [InlineData(3)]
        [InlineData(2)]
        [InlineData(1)]
        public async Task DebugAsync_Good_Code_Run_Until_Breakpoint_CurrentLine_Should_Be_Equails_To(int breakpoint) {
            var session = new CSharpDebuggingSession(new CSharpDebugger());
            var result = await session.DebugAsync(new BuildingTask("test", CSharpSnippets.GoodCode) { Breakpoints = new[] { breakpoint }, IsRunToNextBreakpoint = true });
            result.CurrentLine.Should().Be(breakpoint - 1);
        }

        [Theory]
        [InlineData(6)]
        [InlineData(5)]
        [InlineData(4)]
        [InlineData(3)]
        [InlineData(2)]
        [InlineData(1)]
        public async Task DebugAsync_Good_Code_Run_Until_Breakpoint_WithStep_Over_CurrentLine_Should_Be_Equails_To(int breakpoint) {
            var session = new CSharpDebuggingSession(new CSharpDebugger());
            var result = await session.DebugAsync(new BuildingTask("test", CSharpSnippets.GoodCode) { Breakpoints = new[] { breakpoint }, IsRunToNextBreakpoint = true });
            result = await session.DebugAsync(new BuildingTask("test", CSharpSnippets.GoodCode) { Breakpoints = new[] { breakpoint }, CurrentLine = ++result.CurrentLine });

            result.CurrentLine.Should().Be(breakpoint);
        }

        [Fact]
        public void DebugAsync_Disposed_Should_Throw_ObjectDisposedException() {
            var session = new CSharpDebuggingSession(new CSharpDebugger());
            session.Dispose();
            Func<Task<BuildingResult>> act = () => session.DebugAsync(new BuildingTask(null, null));
            act.Should().ThrowExactly<ObjectDisposedException>();
        }
    }
}
