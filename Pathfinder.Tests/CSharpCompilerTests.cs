using FluentAssertions;
using Pathfinder.Application.BuildingTools;
using Pathfinder.Application.BuildingTools.Background;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Pathfinder.Tests {
    public class CSharpCompilerTests {
        [Fact]
        public void CompileAsync_Null_BuildingTask_Should_Throw_ArgumentNullException() {
            var compiler = new CSharpCompiler();
            Func<Task<byte[]>> act = () => compiler.CompileAsync(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData(""     ,"")]
        [InlineData(" "    , " ")]
        [InlineData(null   , null)]
        [InlineData(""     , null)]
        [InlineData(" "    , null)]
        [InlineData(null   , "")]
        [InlineData(null   , " ")]
        [InlineData(null   , "test")]
        [InlineData(""     , "test")]
        [InlineData(" "    , "test")]
        [InlineData("test" , "")]
        [InlineData("test" , " ")]
        [InlineData("test" , null)]
        public void CompileAsync_Empty_BuildingTask_Should_Throw_InvalidOperationException(string projectName, string sourceCode) {
            var compiler = new CSharpCompiler();
            Func<Task<byte[]>> act = () => compiler.CompileAsync(new BuildingTask(projectName, sourceCode));

            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public async Task CompileAsync_GoodCode_Compilation_Success() {
            var compiler = new CSharpCompiler();
            var bytes    = await compiler.CompileAsync(new BuildingTask("goodCode", CSharpSnippets.GoodCode));

            bytes.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CompileAsync_BadCode_Compilation_Error() {
            var compiler = new CSharpCompiler();
            var bytes = await compiler.CompileAsync(new BuildingTask("badCode", CSharpSnippets.BadCode));

            bytes.Should().BeEmpty();
        }

        [Fact]
        public async Task CompileAsync_ExceptionCode_Compilation_Successfully_Complete() {
            var compiler = new CSharpCompiler();
            var bytes = await compiler.CompileAsync(new BuildingTask("exCode", CSharpSnippets.ExceptionCode));

            bytes.Should().NotBeEmpty();
        }
    }
}
