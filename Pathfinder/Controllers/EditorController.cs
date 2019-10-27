using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Pathfinder.Application.Hubs;
using Pathfinder.Application.Services;
using System.Threading.Tasks;

namespace Pathfinder.Controllers {
    public class EditorController : Controller {
        private readonly IEditorService            _editorService;
        private readonly IHubContext<ProjectsHub>  _projectsHubContext;

        public EditorController(
            IEditorService editorService, 
            IHubContext<ProjectsHub> projectsHubContext) {
            _editorService      = editorService;
            _projectsHubContext = projectsHubContext;
        }

        [Authorize]
        public async Task<IActionResult> Index(string projectName) {
            if (string.IsNullOrWhiteSpace(projectName)) return BadRequest();

            var (joinedTo, project) = await _editorService.Join(projectName, HttpContext.User);
            if (joinedTo) {
                await _projectsHubContext.Clients.All.SendAsync(nameof(ProjectsHub.CollaboratorJoinsProject), project);
                project?.GenerateTemplateCode();

                return View(project);
            } else {
                return NotFound();
            }
        }
    }
}