using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Pathfinder.Application.Hubs;
using Pathfinder.Application.Services;
using Pathfinder.Models;

namespace Pathfinder.Controllers {
    public class HomeController : Controller {
        private readonly IProjectService _projectService;
        private readonly ICollaboratorsService _collaboratorsService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHubContext<ProjectsHub> _projectsHubContext;

        public HomeController(
            IProjectService projectService, 
            ICollaboratorsService collaboratorsService,
            UserManager<IdentityUser> userManager,
            IHubContext<ProjectsHub> projectsHubContext) {
            _projectService       = projectService;
            _collaboratorsService = collaboratorsService;
            _userManager          = userManager;
            _projectsHubContext = projectsHubContext;
        }

        [Authorize]
        public async Task<IActionResult> Index() {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            _ = await _collaboratorsService.SignIn(currentUser);

            var activeRooms = await _projectService.GetProjects();
            return View(activeRooms);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProject([RegularExpression(@"^\s*\S+\s*$")] string projectName) {
            if (!ModelState.IsValid) return View();

            var newProject = new ProjectViewModel { Name = projectName };
            if (await _projectService.Add(newProject))
                await _projectsHubContext.Clients.All.SendAsync(nameof(ProjectsHub.CreateProject), newProject);

            return RedirectToAction(
               actionName: "Index",
               controllerName: "Editor",
               routeValues: new { projectName = newProject.Name });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error() {
            var viewModel = await GetCurrentErrorViewModelAsync();
            return View(viewModel);
        }

        private ValueTask<ErrorViewModel> GetCurrentErrorViewModelAsync() =>
            new ValueTask<ErrorViewModel>(
                new ErrorViewModel { 
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
    }
}