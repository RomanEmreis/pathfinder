﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Pathfinder.Application.BuildingTools;
using Pathfinder.Application.BuildingTools.Background;
using Pathfinder.Application.Services;
using System;
using System.Threading.Tasks;

namespace Pathfinder.Application.Hubs {
    [Authorize]
    public class LiveEditorHub : Hub {
        private readonly IEditorService            _editorService;
        private readonly IHubContext<ProjectsHub>  _projectsHubContext;
        private readonly IBuildingQueue            _buildingQueue;

        public LiveEditorHub(
            IEditorService           editorService,
            IHubContext<ProjectsHub> projectsHubContext,
            IBuildingQueue           buildingQueue) {
            _editorService      = editorService;
            _projectsHubContext = projectsHubContext;
            _buildingQueue      = buildingQueue;
        }

        public async Task CollaboratorJoinsProject(string projectName) => 
            await Groups.AddToGroupAsync(Context.ConnectionId, projectName);

        public async Task CollaboratorLeavesProject(string projectName) {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, projectName);

            var (leftFrom, project) = await _editorService.Leave(projectName, Context.User);
            if (leftFrom) {
                await _projectsHubContext.Clients.All.SendAsync(nameof(ProjectsHub.CollaboratorJoinsProject), project);
            }
        }

        public async Task CodeChanged(string projectName, object changes) =>
            await Clients.GroupExcept(projectName, Context.ConnectionId).SendAsync(nameof(CodeChanged), projectName, changes);

        public async Task CompileProject(BuildingTask buildContext) => 
            await _buildingQueue.QueueAsync(buildContext);

        public async Task StepOver(BuildingTask buildContext) =>
            await _buildingQueue.QueueAsync(buildContext);

        public async Task StepInto(BuildingTask buildContext) =>
            await _buildingQueue.QueueAsync(buildContext);

        public async Task SetBreakpoint(string projectName, object range) =>
            await Clients.GroupExcept(projectName, Context.ConnectionId).SendAsync(nameof(SetBreakpoint), range);

        public async Task RemoveBreakpoint(string projectName, object range, object breakpoint) =>
            await Clients.GroupExcept(projectName, Context.ConnectionId).SendAsync(nameof(RemoveBreakpoint), range, breakpoint);

        public async Task ProjectCompiled(string projectName, BuildingResult buildResult) =>
            await Clients.Group(projectName).SendAsync(nameof(ProjectCompiled), projectName, buildResult);

        public override Task OnConnectedAsync() => base.OnConnectedAsync();

        public override Task OnDisconnectedAsync(Exception exception) => base.OnDisconnectedAsync(exception);
    }
}