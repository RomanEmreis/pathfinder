using Microsoft.AspNetCore.Identity;
using Pathfinder.Models;
using System;
using System.Collections.Generic;

namespace Pathfinder.Application.Entities {
    /// <summary>
    ///     Default <see cref="IProjectCollaborator"/> implementation that uses the <seealso cref="IdentityUser"/> as user contract
    /// </summary>
    public sealed class UserCollaborator : IProjectCollaborator {
        private UserCollaborator() => CollaboratedProjects = new HashSet<ProjectViewModel>();

        public UserCollaborator(IdentityUser identityUser) : this() {
            if (identityUser is null) throw new ArgumentNullException(nameof(identityUser));
            UserName = identityUser.NormalizedUserName;
        }

        public string UserName { get; } = null!;

        public ISet<ProjectViewModel> CollaboratedProjects { get; }

        public void Collaborate(ProjectViewModel project) {
            if (project is null) throw new ArgumentNullException(nameof(project));

            if (CollaboratedProjects.Add(project)) {
                project.Collaborators.Add(this);
            }
        }

        public void Leave(ProjectViewModel project) {
            if (project is null) throw new ArgumentNullException(nameof(project));

            if (CollaboratedProjects.Remove(project)) {
                project.Collaborators.Remove(this);
            }
        }
    }
}