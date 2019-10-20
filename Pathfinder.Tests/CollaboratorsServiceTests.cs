using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.Entities;
using Pathfinder.Application.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Pathfinder.Tests {
    public class CollaboratorsServiceTests {
        [Fact]
        public void SignIn_With_Null_Parameter_Should_Throw_ArgumentNullException() {
            var service = new CollaboratorsService();
            Func<ValueTask<IProjectCollaborator?>> act = () => service.SignIn(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void SignOut_With_Null_Parameter_Should_Throw_ArgumentNullException() {
            var service = new CollaboratorsService();
            Func<ValueTask> act = () => service.SignOut(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("test user")]
        public async Task SignIn_With_Not_Null_Parameter_Collaborators_Should_Contain_It(string userName) {
            var service = new CollaboratorsService();
            var identityUser = new IdentityUser { Id = userName, NormalizedUserName = userName };

            var collaborator = await service.SignIn(identityUser);

            service.Collaborators.Should().Contain(collaborator);
        }

        [Theory]
        [InlineData("test user")]
        public async Task SignIn_Twice_The_Same_User_Collaborators_Should_Have_Count_1(string userName) {
            var service = new CollaboratorsService();
            var identityUser = new IdentityUser { Id = userName, NormalizedUserName = userName };

            var collaborator1 = await service.SignIn(identityUser);
            var collaborator2 = await service.SignIn(identityUser);

            service.Collaborators.Should().HaveCount(1);
            collaborator1.Should().NotBeNull();
            collaborator2.Should().BeNull();
        }

        [Theory]
        [InlineData("test user")]
        public async Task SignOut_With_Not_Null_Parameter_Collaborators_Should_Not_Contain_It(string userName) {
            var service = new CollaboratorsService();
            var identityUser = new IdentityUser { Id = userName, NormalizedUserName = userName };

            var collaborator = await service.SignIn(identityUser);
            await service.SignOut(identityUser);

            service.Collaborators.Should().NotContain(collaborator);
        }

        [Fact]
        public void TryGetCollaborator_With_Null_Parameter_Should_Throw_ArgumentNullException() {
            var service = new CollaboratorsService();
            Action act = () => service.TryGetCollaborator(null, out var projectCollaborator);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("sdfs")]
        public void TryGetCollaborator_With_Invalid_Parameter_Should_Return_Null(string id) {
            var service = new CollaboratorsService();
            var result = service.TryGetCollaborator(id, out var projectCollaborator);

            result.Should().BeFalse();
            projectCollaborator.Should().BeNull();
        }
    }
}
