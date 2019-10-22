using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Pathfinder.Application.BuildingTools;
using Pathfinder.Application.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Pathfinder.Application.Hubs {
    [Authorize]
    public class LiveEditorHub : Hub {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IProjectService _projectsService;
        private readonly ICollaboratorsService _collaboratorsService;
        private readonly IHubContext<ProjectsHub> _projectsHubContext;

        public LiveEditorHub(
            IProjectService projectsService,
            ICollaboratorsService collaboratorsService,
            UserManager<IdentityUser> userManager,
            IHubContext<ProjectsHub> projectsHubContext) {
            _projectsService = projectsService;
            _userManager = userManager;
            _collaboratorsService = collaboratorsService;
            _projectsHubContext = projectsHubContext;
        }

        public async Task CollaboratorJoinsProject(string projectName) => 
            await Groups.AddToGroupAsync(Context.ConnectionId, projectName);

        public async Task CollaboratorLeavesProject(string projectName) {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectName);

            if (_projectsService.TryGetProject(projectName, out var project)) {
                var currentUser = await _userManager.GetUserAsync(Context.User);
                if (_collaboratorsService.TryGetCollaborator(currentUser.Id, out var collaborator))
                    await _projectsService.Leave(project.Name, collaborator);

                await _projectsHubContext.Clients.All.SendAsync(nameof(ProjectsHub.CollaboratorJoinsProject), project);
            }
        }

        public async Task CodeChanged(string projectName, object code) =>
            await Clients.GroupExcept(projectName, Context.ConnectionId).SendAsync(nameof(CodeChanged), projectName, code);

        public async Task CompileProject(string projectName, string code) {
            using var sw = new StringWriter();

            Console.SetOut(sw);
            Console.SetError(sw);

            var compiler              = new Compiler();
            var assemblyBytes         = await Task.Run(() => compiler.Compile(projectName, code));

            var runner                = new Runner();
            var buildResult           = await Task.Run(() => runner.Execute(assemblyBytes, Array.Empty<string>()));

            buildResult.ResultMessage = sw.ToString();

            await Clients.Group(projectName).SendAsync(nameof(ProjectCompiled), projectName, buildResult);
        }

        public async Task ProjectCompiled(string projectName, BuildingResult buildResult) =>
            await Clients.Group(projectName).SendAsync(nameof(ProjectCompiled), projectName, buildResult);

        public override Task OnConnectedAsync() => base.OnConnectedAsync();

        public override Task OnDisconnectedAsync(Exception exception) => base.OnDisconnectedAsync(exception);
    }
}
