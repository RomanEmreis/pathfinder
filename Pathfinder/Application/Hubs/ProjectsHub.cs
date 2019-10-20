using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Pathfinder.Models;
using System;
using System.Threading.Tasks;

namespace Pathfinder.Application.Hubs {
    [Authorize]
    public sealed class ProjectsHub : Hub {
        public async Task CreateProject(ProjectViewModel project) => 
            await Clients.All.SendAsync(nameof(CreateProject), project);

        public async Task CollaboratorJoinsProject(ProjectViewModel project) => 
            await Clients.All.SendAsync(nameof(CollaboratorJoinsProject), project).ConfigureAwait(false);

        public override Task OnConnectedAsync() => base.OnConnectedAsync();

        public override Task OnDisconnectedAsync(Exception exception) => base.OnDisconnectedAsync(exception);
    }
}