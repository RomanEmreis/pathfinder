using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Pathfinder.Application.BuildingTools;
using Pathfinder.Application.Services;
using Pathfinder.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pathfinder.Tests {
    public class EditorServiceTests {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Join_Null_Or_Empty_Or_Whitespace_Should_Throw_ArgumentException(string projectName) {
            Func<Task> act = async () => await new EditorService(null, null, null, null).Join(projectName, null);
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Leave_Null_Or_Empty_Or_Whitespace_Should_Throw_ArgumentException(string projectName) {
            Func<Task> act = async () => await new EditorService(null, null, null, null).Leave(projectName, null);
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Join_Principal_Should_Throw_ArgumentNullException() {
            Func<Task> act = async () => await new EditorService(null, null, null, null).Join("test", null);
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Leave_Principal_Should_Throw_ArgumentNullException() {
            Func<Task> act = async () => await new EditorService(null, null, null, null).Leave("test", null);
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public async Task Join_Not_Existed_Project_Should_Returns_False_And_Null() {
            var projectService       = new ProjectService();
            var esitorService = new EditorService(projectService, null, null, null);
            var (joinedTo, project) = await esitorService.Join("test", new ClaimsPrincipal());
            joinedTo.Should().BeFalse();
            project.Should().BeNull();
        }

        [Fact]
        public async Task Leave_Not_Existed_Project_Should_Returns_False_And_Null() {
            var projectService = new ProjectService();
            var esitorService = new EditorService(projectService, null, null, null);
            var (joinedTo, project) = await esitorService.Leave("test", new ClaimsPrincipal());
            joinedTo.Should().BeFalse();
            project.Should().BeNull();
        }
    }
}
