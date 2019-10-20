using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Entities;
using Pathfinder.Models;
using System;
using Xunit;

namespace Pathfinder.Tests {
    public class UserCollaboratorTests {
        [Fact]
        public void Init_With_Null_Parameter_Should_Throw_ArgumentNullException() {
            Action act = () => new UserCollaborator(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("test user 1")]
        public void Init_With_Not_Null_Parameter_UserName_Should_Be_Equals_To(string userName) {
            var collaborator = CreateFakeUser(userName);

            collaborator.UserName.Should().Be(userName);
        }

        [Theory]
        [InlineData("test user 1")]
        public void Collaborate_With_Null_Parameter_Should_Throw_ArgumentNullException(string userName) {
            var collaborator = CreateFakeUser(userName);

            Action act = () => collaborator.Collaborate(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("test user 1", "testProject1")]
        public void Collaborate_With_Not_Null_Parameter_CollaboratedProjects_Should_Contain_It(string userName, string projectName) {
            var collaborator = CreateFakeUser(userName);
            var project      = CreateFakeProject(projectName);

            collaborator.Collaborate(project);

            collaborator.CollaboratedProjects.Should().Contain(project);
        }

        [Theory]
        [InlineData("test user 1", "testProject1")]
        public void Collaborate_With_Not_Null_Parameter_Project_Collaborators_Should_Contain_Collaborator(string userName, string projectName) {
            var collaborator = CreateFakeUser(userName);
            var project = CreateFakeProject(projectName);

            collaborator.Collaborate(project);

            project.Collaborators.Should().Contain(collaborator);
        }

        [Theory]
        [InlineData("test user 1")]
        public void Leave_With_Null_Parameter_Should_Throw_ArgumentNullException(string userName) {
            var collaborator = CreateFakeUser(userName);

            Action act = () => collaborator.Leave(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("test user 1", "testProject1")]
        public void Leave_With_Not_Null_Parameter_CollaboratedProjects_Should_Not_Contain_It(string userName, string projectName) {
            var collaborator = CreateFakeUser(userName);
            var project = CreateFakeProject(projectName);

            collaborator.Collaborate(project);
            collaborator.Leave(project);

            collaborator.CollaboratedProjects.Should().NotContain(project);
        }

        [Theory]
        [InlineData("test user 1", "testProject1")]
        public void Leave_With_Not_Null_Parameter_Project_Collaborators_Should_Not_Contain_Collaborator(string userName, string projectName) {
            var collaborator = CreateFakeUser(userName);
            var project = CreateFakeProject(projectName);

            collaborator.Collaborate(project);
            collaborator.Leave(project);

            project.Collaborators.Should().NotContain(collaborator);
        }

        private UserCollaborator CreateFakeUser(string userName) {
            var identityUser = new IdentityUser { NormalizedUserName = userName };
            return new UserCollaborator(identityUser);
        }

        private ProjectViewModel CreateFakeProject(string projectName) => new ProjectViewModel { Name = projectName };
    }
}
