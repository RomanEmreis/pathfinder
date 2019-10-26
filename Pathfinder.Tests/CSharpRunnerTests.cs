using FluentAssertions;
using Pathfinder.Application.BuildingTools;
using Pathfinder.Application.BuildingTools.Background;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Pathfinder.Tests {
    public class CSharpRunnerTests {
        [Fact]
        public async Task ExecuteAsync_Empty_Assembly_Bytes_Should_Return_False_Build_Result() {
            var runner      = new CSharpRunner();
            var buildResult = await runner.ExecuteAsync(Array.Empty<byte>(), Array.Empty<string>());

            buildResult.Success.Should().BeFalse();
        }

        [Fact]
        public async Task ExecuteAsync_GoodCode_Compilation_Success() {
            var compiler = new CSharpCompiler();
            var bytes = await compiler.CompileAsync(new BuildingTask("goodCode", CSharpSnippets.GoodCode));
            var runner = new CSharpRunner();
            var buildResult = await runner.ExecuteAsync(bytes, Array.Empty<string>());

            buildResult.Success.Should().BeTrue();
        }

        [Fact]
        public async Task ExecuteAsync_ExceptionCode_Compilation_Successfully_Complete() {
            var compiler = new CSharpCompiler();
            var bytes = await compiler.CompileAsync(new BuildingTask("exCode", CSharpSnippets.ExceptionCode));
            var runner = new CSharpRunner();
            var buildResult = await runner.ExecuteAsync(bytes, Array.Empty<string>());

            buildResult.Success.Should().BeFalse();
        }
    }
}
