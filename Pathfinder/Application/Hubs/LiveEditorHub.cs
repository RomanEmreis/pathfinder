using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Pathfinder.Application.Hubs {
    [Authorize]
    public class LiveEditorHub : Hub {
        public async Task CodeChanged(object code) =>
            await Clients.Others.SendAsync(nameof(CodeChanged), code);

        public override Task OnConnectedAsync() => base.OnConnectedAsync();

        public override Task OnDisconnectedAsync(Exception exception) => base.OnDisconnectedAsync(exception);
    }
}
