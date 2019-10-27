using Microsoft.AspNetCore.Identity;
using Pathfinder.Application.BuildingTools;
using Pathfinder.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pathfinder.Application.Services {
    public sealed class EditorService : IEditorService {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IDebuggingSessions        _debuggingSessions;
        private readonly IProjectService           _projectsService;
        private readonly ICollaboratorsService     _collaboratorsService;

        public EditorService(
            IProjectService           projectsService, 
            ICollaboratorsService     collaboratorsService,
            UserManager<IdentityUser> userManager,
            IDebuggingSessions        debuggingSessions) {
            _projectsService      = projectsService;
            _userManager          = userManager;
            _debuggingSessions    = debuggingSessions;
            _collaboratorsService = collaboratorsService;
        }

        public async ValueTask<(bool, ProjectViewModel?)> Join(string projectName, ClaimsPrincipal principal) {
            if (string.IsNullOrWhiteSpace(projectName)) throw new ArgumentException(ErrorConstants.MissingProjectName);
            if (principal is null)                      throw new ArgumentNullException(nameof(principal));

            if (_projectsService.TryGetProject(projectName, out var project)) {
                var currentUser = await _userManager.GetUserAsync(principal);

                if (_collaboratorsService.TryGetCollaborator(currentUser.Id, out var collaborator)) {
                    await _projectsService.Join(project.Name, collaborator);
                    return (true, project);
                }

                return (false, project);
            } else {
                return (false, default);
            }
        }

        public async ValueTask<(bool, ProjectViewModel?)> Leave(string projectName, ClaimsPrincipal principal) {
            if (string.IsNullOrWhiteSpace(projectName)) throw new ArgumentException(ErrorConstants.MissingProjectName);
            if (principal is null)                      throw new ArgumentNullException(nameof(principal));

            if (_projectsService.TryGetProject(projectName, out var project)) {
                var currentUser = await _userManager.GetUserAsync(principal);

                bool leftFrom;
                if (leftFrom = _collaboratorsService.TryGetCollaborator(currentUser.Id, out var collaborator)) {
                    await _projectsService.Leave(project.Name, collaborator);
                }

                if (!project.HasCollaborators)
                    await _debuggingSessions.EndSession(projectName);

                return (leftFrom, project);
            }
            return (false, default);
        }
    }
}
