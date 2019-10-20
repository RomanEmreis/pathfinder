using FluentAssertions;
using Pathfinder.Application.Entities;
using Pathfinder.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pathfinder.Tests {
    public class ProjectServiceTests {
        [Fact]
        public async Task GetProjects_Should_Not_Returns_Null() {
            var service  = new ProjectService();
            var projects = await service.GetProjects();

            projects.Should().NotBeNull();
        }

        [Fact]
        public void Add_With_Null_Parameter_Should_Throw_ArgumentNullException() {
            var service = new ProjectService();
            Func<ValueTask<bool>> act = () => service.Add(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public async Task Add_With_Not_Null_Parameter_GetProjects_Should_Have_Count_1() {
            var service = new ProjectService();
            await service.Add(new Models.ProjectViewModel { Name = "test" });

            var projects = await service.GetProjects();

            projects.Projects.Should().HaveCount(1);
        }

        [Fact]
        public void Join_With_Null_Collaborator_Should_Throw_ArgumentNullException() {
            var service = new ProjectService();
            Func<ValueTask> act = () => service.Join(null, null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("testProject", "test user")]
        public async Task Join_Specific_Project_Collaborator_Should_Collaborate(string projectName, string userName) {
            var service      = new ProjectService();
            var project      = new Models.ProjectViewModel { Name = projectName };
            var collaborator = new UserCollaborator(new Microsoft.AspNetCore.Identity.IdentityUser { NormalizedUserName = userName });

            await service.Add(project);
            await service.Join(project.Name, collaborator);

            project.Collaborators.Should().Contain(collaborator);
            collaborator.CollaboratedProjects.Should().Contain(project);
        }

        [Fact]
        public void Leave_With_Null_Collaborator_Should_Throw_ArgumentNullException() {
            var service = new ProjectService();
            Func<ValueTask> act = () => service.Leave(null, null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("testProject", "test user")]
        public async Task Leave_Specific_Project_Collaborator_Should_Not_Collaborate(string projectName, string userName) {
            var service = new ProjectService();
            var project = new Models.ProjectViewModel { Name = projectName };
            var collaborator = new UserCollaborator(new Microsoft.AspNetCore.Identity.IdentityUser { NormalizedUserName = userName });

            await service.Add(project);
            await service.Join(project.Name, collaborator);
            await service.Leave(project.Name, collaborator);

            project.Collaborators.Should().NotContain(collaborator);
            collaborator.CollaboratedProjects.Should().NotContain(project);
        }

        [Fact]
        public void Join_With_Null_ProjectName_Should_Throw_ArgumentNullException() {
            var service = new ProjectService();
            Func<ValueTask> act = () => service.Join(null, new UserCollaborator(new Microsoft.AspNetCore.Identity.IdentityUser()));

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Leave_With_Null_ProjectName_Should_Throw_ArgumentNullException() {
            var service = new ProjectService();
            Func<ValueTask> act = () => service.Leave(null, new UserCollaborator(new Microsoft.AspNetCore.Identity.IdentityUser()));

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void TryGetProject_With_Null_Parameter_Should_Throw_ArgumentNullException() {
            var service = new ProjectService();
            Action act = () => service.TryGetProject(null, out var project);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("sdfs")]
        public void TryGetProject_With_Invalid_Parameter_Should_Return_Null(string id) {
            var service = new ProjectService();
            var result = service.TryGetProject(id, out var projectCollaborator);

            result.Should().BeFalse();
            projectCollaborator.Should().BeNull();
        }
    }
}
