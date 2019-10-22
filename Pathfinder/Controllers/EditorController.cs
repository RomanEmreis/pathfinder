using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Pathfinder.Application.Hubs;
using Pathfinder.Application.Services;
using System.Threading.Tasks;

namespace Pathfinder.Controllers {
    public class EditorController : Controller {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IProjectService           _projectsService;
        private readonly ICollaboratorsService     _collaboratorsService;
        private readonly IHubContext<ProjectsHub>  _projectsHubContext;

        public EditorController(
            IProjectService projectsService, 
            ICollaboratorsService collaboratorsService,
            UserManager<IdentityUser> userManager,
            IHubContext<ProjectsHub> projectsHubContext) {
            _projectsService      = projectsService;
            _userManager          = userManager;
            _collaboratorsService = collaboratorsService;
            _projectsHubContext   = projectsHubContext;
        }

        [Authorize]
        public async Task<IActionResult> Index(string projectName) {
            if (string.IsNullOrWhiteSpace(projectName)) return BadRequest();

            if (_projectsService.TryGetProject(projectName, out var project)) {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                if (_collaboratorsService.TryGetCollaborator(currentUser.Id, out var collaborator))
                    await _projectsService.Join(project.Name, collaborator);

                await _projectsHubContext.Clients.All.SendAsync(nameof(ProjectsHub.CollaboratorJoinsProject), project);
            } else {
                return BadRequest();
            }

            project.GenerateTemplateCode();
            return View(project);
        }
    }
}